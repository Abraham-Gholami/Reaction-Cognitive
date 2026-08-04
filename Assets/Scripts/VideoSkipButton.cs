using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Skips the intro story video. Lives on a button under the video canvas, so it is
// only on screen while the video plays.
//
// The button lays itself out rather than relying on serialized RectTransform values,
// so it stays put in the bottom corner whatever the aspect ratio, and wires its own
// onClick instead of needing an inspector-assigned event.
[ExecuteAlways]
[RequireComponent(typeof(Button))]
public class VideoSkipButton : MonoBehaviour
{
    [SerializeField] string caption = "رد کردن";
    [SerializeField] Vector2 size = new Vector2(300f, 120f);
    [SerializeField] Vector2 margin = new Vector2(60f, 80f);

    void OnEnable()
    {
        Layout();
    }

    void Awake()
    {
        if (Application.isPlaying)
            GetComponent<Button>().onClick.AddListener(Skip);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        Layout();
    }
#endif

    void Layout()
    {
        var rt = transform as RectTransform;
        if (rt == null) return;

        // bottom-right corner of the video canvas
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 0f);
        rt.sizeDelta = size;
        rt.anchoredPosition = new Vector2(-margin.x, margin.y);
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        var label = GetComponentInChildren<TMP_Text>(true);
        if (label == null) return;

        var lrt = label.rectTransform;
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;
        lrt.localScale = Vector3.one;
        lrt.localRotation = Quaternion.identity;

        // Assigning through TMP_Text still reaches RTLTextMeshPro's overridden setter,
        // so the Persian caption gets shaped and reordered correctly.
        if (label.text != caption) label.text = caption;
        label.alignment = TextAlignmentOptions.Center;
    }

    void Skip()
    {
        if (RandomButtonGenerator.Instance != null)
            RandomButtonGenerator.Instance.SkipVideo();
    }
}
