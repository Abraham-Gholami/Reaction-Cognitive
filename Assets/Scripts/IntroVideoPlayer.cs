using UnityEngine;
using UnityEngine.Video;

// Plays the intro story video on scene load, before the menu.
// - Skipped if the player turned the "Story" setting off.
// - Draws a pulsing, icon-only Skip button in the TOP-RIGHT (via OnGUI, so it
//   always renders over the camera-plane video). Tap it or let the video end to
//   reveal the menu.
public class IntroVideoPlayer : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject menu;     // hidden during the video, shown after
    [SerializeField] Texture2D skipIcon;  // top-right skip arrow

    bool playing;
    bool finished;
    Texture2D circleTex;

    void Start()
    {
        bool wantStory = SettingsManager.Instance == null || SettingsManager.Instance.useStory;
        bool canPlay = wantStory && videoPlayer != null && videoPlayer.clip != null;

        if (!canPlay)
        {
            Finish();
            return;
        }

        if (menu != null) menu.SetActive(false);
        playing = true;
        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp) => Finish();

    public void Skip() => Finish();

    void Finish()
    {
        if (finished) return;
        finished = true;
        playing = false;

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
            videoPlayer.Stop();
        }
        if (menu != null) menu.SetActive(true);
    }

    void EnsureCircle()
    {
        if (circleTex != null) return;
        int s = 128;
        circleTex = new Texture2D(s, s, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        Vector2 c = new Vector2((s - 1) * 0.5f, (s - 1) * 0.5f);
        float r = s * 0.5f;
        for (int y = 0; y < s; y++)
        {
            for (int x = 0; x < s; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c) / r;
                float a = Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(0.78f, 1f, d)); // soft edge
                circleTex.SetPixel(x, y, new Color(1f, 1f, 1f, a * 0.22f));          // frosted glass
            }
        }
        circleTex.Apply();
    }

    void OnGUI()
    {
        if (!playing || skipIcon == null) return;
        EnsureCircle();

        float pulse = 1f + 0.12f * Mathf.Sin(Time.unscaledTime * 4.2f);
        float baseSize = Screen.height * 0.085f;
        float size = baseSize * pulse;
        float margin = Screen.height * 0.03f;
        float cx = Screen.width - margin - baseSize * 0.5f;
        float cy = margin + baseSize * 0.5f;

        Color prev = GUI.color;

        // frosted circle backing
        var circleRect = new Rect(cx - size * 0.5f, cy - size * 0.5f, size, size);
        GUI.DrawTexture(circleRect, circleTex);

        // arrow, centred, keeping aspect (~52% of the circle)
        float aw = size * 0.42f;
        float ah = aw * skipIcon.height / Mathf.Max(1, skipIcon.width);
        if (ah > size * 0.55f) { ah = size * 0.55f; aw = ah * skipIcon.width / Mathf.Max(1, skipIcon.height); }
        var arrowRect = new Rect(cx - aw * 0.5f, cy - ah * 0.5f, aw, ah);
        GUI.DrawTexture(arrowRect, skipIcon, ScaleMode.ScaleToFit);

        GUI.color = prev;

        // generous invisible hit area
        if (GUI.Button(new Rect(cx - baseSize * 0.7f, cy - baseSize * 0.7f, baseSize * 1.4f, baseSize * 1.4f),
                       GUIContent.none, GUIStyle.none))
            Skip();
    }
}
