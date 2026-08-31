using UnityEngine;
using UnityEngine.UI;

// TEMPORARY test harness: drains the oxygen tank on a loop so the emptying can be
// watched without playing a whole level.
//
// This lives on an ALWAYS-ACTIVE object (the Canvas), not on the tank itself:
// RandomButtonGenerator.Start() deactivates MainGamePlay at the menu, and a component
// on a deactivated object never gets Update, so a test attached to the tank would
// never run when you press Play. Instead it forces the tank's chain active itself.
//
// Editor-only - it removes itself in a build - and does nothing unless `run` is ticked.
[ExecuteAlways]
public class TankDrainTest : MonoBehaviour
{
    [Tooltip("Tick to loop the drain. Untick to hand the tank back to normal gameplay.")]
    public bool run = true;

    [Tooltip("Seconds for a full tank to empty.")]
    public float secondsToEmpty = 6f;

    [Tooltip("The tank's Slider. Driving this exercises the real path: ProgressBar\n"
           + "converts slider value into the fill image's fillAmount every frame.")]
    public Slider slider;

    [Tooltip("Fallback if no Slider is set - drives the fill image directly.")]
    public Image fill;

    [Tooltip("The tank root - forced visible while testing.")]
    public GameObject tankRoot;

    float t;

    void OnEnable() { t = 0f; }

    void Update()
    {
#if UNITY_EDITOR
        if (!run || secondsToEmpty <= 0f) return;
        if (slider == null && fill == null) return;

        // the menu hides MainGamePlay, so make the tank's whole chain visible
        if (tankRoot != null)
        {
            var tr = tankRoot.transform;
            while (tr != null)
            {
                if (!tr.gameObject.activeSelf) tr.gameObject.SetActive(true);
                tr = tr.parent;
            }
        }

        // Overwrites whatever ProgressBar sets, so untick `run` to judge real gameplay.
        t += Application.isPlaying ? Time.deltaTime : 0.016f;
        var k = Mathf.Clamp01(1f - (t % secondsToEmpty) / secondsToEmpty);
        if (slider != null)
            // Drive the slider, not the image: ProgressBar.SyncFill rewrites fillAmount
            // from the slider every frame, so setting the image alone is overwritten.
            slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, k);
        else
            fill.fillAmount = k;
#else
        Destroy(this);
#endif
    }
}
