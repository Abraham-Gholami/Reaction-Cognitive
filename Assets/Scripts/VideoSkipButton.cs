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

        // The video canvas is rendered by VideoCam, whose culling mask covers only that
        // canvas's layer. A button created at the scene root keeps the Default layer and
        // is silently culled — visible in the hierarchy, drawn nowhere. Inherit the
        // canvas's layer for this object and everything under it.
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            var layer = canvas.gameObject.layer;
            gameObject.layer = layer;
            foreach (var t in GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;
        }

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

        // The counters' font (Lalezar) ships a static, digits-only atlas, so a Persian
        // caption on it renders as nothing at all — a silent blank button. Say so loudly
        // rather than leaving someone to wonder why the label is invisible.
        if (label.font != null && !label.font.HasCharacters(caption))
            Debug.LogWarning("VideoSkipButton: font '" + label.font.name +
                             "' has no glyphs for \"" + caption + "\" - the label will be blank.");
    }

    void Skip()
    {
        if (RandomButtonGenerator.Instance != null)
            RandomButtonGenerator.Instance.SkipVideo();
    }
}
