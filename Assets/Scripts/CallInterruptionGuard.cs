using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Detects when the OS takes our audio away — an incoming call, an answered call, or
// the app being backgrounded — and latches the test into a paused state until an
// operator taps Resume.
//
// Android hands exclusive audio focus to telephony; nothing an app does can keep the
// game audible through a call. What we CAN do is make sure no audio stimulus is
// presented while the child cannot hear it, and record that the trial was spoiled.
//
// Detection uses AudioManager.getMode(), which needs no permission and, unlike
// OnApplicationPause, also catches a phone ringing while the game stays in the
// foreground (a heads-up call notification never backgrounds the app).
public class CallInterruptionGuard : MonoBehaviour
{
    const int MODE_RINGTONE = 1;
    const int MODE_IN_CALL = 2;
    const int MODE_IN_COMMUNICATION = 3;

    public static CallInterruptionGuard Instance { get; private set; }

    // True from the moment the OS takes our audio until an operator resumes.
    public static bool Paused { get; private set; }

    // realtimeSinceStartup of the most recent interruption; -1 if there has been none.
    // Gameplay compares this against a trial's start time to know if it was spoiled.
    public static float LastInterruptionTime { get; private set; } = -1f;

    // Raised the moment an interruption starts, so a trial in flight can be voided.
    public static event Action Interrupted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("CallInterruptionGuard");
        Instance = go.AddComponent<CallInterruptionGuard>();
        DontDestroyOnLoad(go);
    }

    AndroidJavaObject audioManager;
    float nextPoll;
    GameObject overlay;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Paused = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                audioManager = activity.Call<AndroidJavaObject>("getSystemService", "audio");
        }
        catch (Exception e)
        {
            Debug.LogWarning("CallInterruptionGuard: no AudioManager (" + e.Message + ")");
        }
#endif
    }

    // Set while the OS still owns the audio; cleared only when getMode() reports
    // MODE_NORMAL again. Without this, tapping Resume the instant a call ended re-read
    // MODE_IN_CALL (Android holds it for a second or two after hang-up) and the overlay
    // popped straight back up.
    bool audioStillBusy;

    void Update()
    {
        if (audioManager == null) return;
        // Four polls a second is plenty to catch a ring and costs nothing measurable.
        if (Time.realtimeSinceStartup < nextPoll) return;
        nextPoll = Time.realtimeSinceStartup + 0.25f;

        int mode;
        try { mode = audioManager.Call<int>("getMode"); }
        catch { return; }

        bool busy = mode == MODE_RINGTONE || mode == MODE_IN_CALL || mode == MODE_IN_COMMUNICATION;
        audioStillBusy = busy;
        if (busy) Interrupt();
    }

    // True once the telephony stack has released the audio, so Resume can be honoured.
    public static bool CanResume
    {
        get { return Instance == null || !Instance.audioStillBusy; }
    }

    // Backstop for a full-screen call UI, or the operator switching apps.
    // NOT used for screen-off: the display sleeping is not an audio interruption, and
    // latching on it made a long video or a quiet run of no-go trials pause the test.
    void OnApplicationPause(bool paused)
    {
        if (paused && audioStillBusy) Interrupt();
    }

    public void Interrupt()
    {
        LastInterruptionTime = Time.realtimeSinceStartup;
        if (Paused) return;
        Paused = true;
        // Silence our own audio too, so a stimulus clip that was already playing does
        // not resume half-way through behind the overlay.
        AudioListener.pause = true;
        ShowOverlay(true);
        var handler = Interrupted;
        if (handler != null) handler();
    }

    // Wired to the Resume button — the operator confirms the child is ready again.
    public void Resume()
    {
        if (!Paused) return;
        if (!CanResume) return;      // telephony has not released the audio yet
        Paused = false;
        AudioListener.pause = false;
        ShowOverlay(false);
    }

    void ShowOverlay(bool show)
    {
        if (overlay == null && show) BuildOverlay();
        if (overlay != null) overlay.SetActive(show);
    }

    [SerializeField] string message = "آزمون متوقف شد";
    [SerializeField] string buttonLabel = "ادامه";

    void BuildOverlay()
    {
        overlay = new GameObject("InterruptionOverlay");
        overlay.transform.SetParent(transform, false);

        var canvas = overlay.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;      // above every gameplay canvas
        var scaler = overlay.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        overlay.AddComponent<GraphicRaycaster>();

        var dim = NewRect("Dim", overlay.transform);
        Stretch(dim);
        var dimImage = dim.gameObject.AddComponent<Image>();
        dimImage.color = new Color(0f, 0f, 0f, 0.85f);   // also blocks taps on the game

        var label = CloneSceneText(dim, message, 90f, new Vector2(0f, 160f), new Vector2(900f, 400f));

        var button = NewRect("Resume", dim);
        button.anchorMin = button.anchorMax = new Vector2(0.5f, 0.5f);
        button.anchoredPosition = new Vector2(0f, -220f);
        button.sizeDelta = new Vector2(520f, 170f);
        var buttonImage = button.gameObject.AddComponent<Image>();
        buttonImage.color = new Color(0.15f, 0.6f, 0.9f, 1f);
        var press = button.gameObject.AddComponent<Button>();
        press.targetGraphic = buttonImage;
        press.onClick.AddListener(Resume);
        CloneSceneText(button, buttonLabel, 70f, Vector2.zero, new Vector2(520f, 170f));

        if (label == null)
            Debug.LogWarning("CallInterruptionGuard: no TMP text found to clone; overlay has no caption.");
    }

    // Clones a TMP_Text already used in the scene so the pause screen inherits the
    // project's Persian font and right-to-left handling without this script having to
    // reference RTLTMPro directly.
    TMP_Text CloneSceneText(Transform parent, string text, float size, Vector2 pos, Vector2 sizeDelta)
    {
        TMP_Text source = null;
        foreach (var candidate in Resources.FindObjectsOfTypeAll<TMP_Text>())
        {
            if (candidate.gameObject.scene.IsValid() && candidate.font != null) { source = candidate; break; }
        }
        if (source == null) return null;

        var clone = Instantiate(source, parent);
        clone.gameObject.name = "Label";
        clone.gameObject.SetActive(true);
        foreach (Transform child in clone.transform) Destroy(child.gameObject);

        // The source may carry buttons, layout groups or gameplay scripts — keep only
        // what is needed to draw text.
        foreach (var component in clone.GetComponents<Component>())
        {
            if (component is Transform || component is CanvasRenderer || component is TMP_Text) continue;
            Destroy(component);
        }

        var rt = clone.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = sizeDelta;
        rt.localScale = Vector3.one;

        clone.enableAutoSizing = false;
        clone.fontSize = size;
        clone.alignment = TextAlignmentOptions.Center;
        clone.color = Color.white;
        clone.text = text;
        return clone;
    }

    static RectTransform NewRect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
