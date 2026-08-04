using UnityEngine;

// Session-level protections for an unattended test running on a tablet handed to a child:
//
//  * keeps the screen awake — the system timeout is typically 30s, and the intro video or
//    a run of no-go trials can easily exceed that; a sleeping screen also used to be
//    misread as a phone-call interruption.
//  * swallows the Android Back button/gesture. By default it finishes the activity, and
//    since results are only uploaded at the very end that silently discarded the session.
//
// Installs itself, so neither scene needs wiring.
public class SessionGuard : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (instance != null) return;
        var go = new GameObject("SessionGuard");
        instance = go.AddComponent<SessionGuard>();
        DontDestroyOnLoad(go);
    }

    static SessionGuard instance;

    // Set true once the test is finished and uploaded, so the operator can still leave.
    public static bool AllowQuit;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.wantsToQuit += OnWantsToQuit;
    }

    void OnDestroy()
    {
        if (instance == this) Application.wantsToQuit -= OnWantsToQuit;
    }

    // Returning false vetoes the quit. This is the only reliable hook: the Back
    // button/gesture finishes the activity, and a KeyCode.Escape handler alone does not
    // stop that.
    static bool OnWantsToQuit()
    {
        if (AllowQuit) return true;
        ScreenDebug.Instance?.Debug("Quit blocked - test in progress");
        return false;
    }

    void Update()
    {
        // Re-assert: some OEM power-saving modes reset this when the app is resumed.
        if (Screen.sleepTimeout != SleepTimeout.NeverSleep)
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
