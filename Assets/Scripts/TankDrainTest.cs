using UnityEngine;
using UnityEngine.UI;

// TEMPORARY test harness: drains the oxygen tank on a loop so the emptying can be
// watched without playing a whole level. Editor-only - it removes itself in a build,
// and it does nothing unless `run` is ticked.
[ExecuteAlways]
public class TankDrainTest : MonoBehaviour
{
    [Tooltip("Tick to loop the drain. Untick to leave the tank wherever it is.")]
    public bool run = true;

    [Tooltip("Seconds for a full tank to empty.")]
    public float secondsToEmpty = 6f;

    [Tooltip("The Fill image of the tank (Tank/Fill Area/Fill).")]
    public Image fill;

    float t;

    void Reset()
    {
        var f = transform.Find("Fill Area/Fill");
        if (f != null) fill = f.GetComponent<Image>();
    }

    void OnEnable()
    {
        if (fill == null) Reset();
        t = 0f;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!run || fill == null || secondsToEmpty <= 0f) return;
        // Real gameplay drives fillAmount from ProgressBar; while this test runs it
        // simply overwrites it, so untick `run` before judging actual gameplay.
        t += Application.isPlaying ? Time.deltaTime : 0.016f;
        fill.fillAmount = Mathf.Clamp01(1f - (t % secondsToEmpty) / secondsToEmpty);
#else
        // never ships
        Destroy(this);
#endif
    }
}
