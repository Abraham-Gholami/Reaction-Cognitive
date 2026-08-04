using UnityEngine;
using UnityEngine.UI;

// Skips the intro story video. Lives on a button under the video canvas, so it is
// only on screen while the video plays.
//
// Deliberately icon-only: an earlier text version rendered nothing at all, first
// because the cloned label inherited a digits-only font atlas and then because the
// TMP mesh was never rebuilt. A plain sprite has no font asset, no atlas and no mesh
// regeneration to go wrong.
//
// The button lays itself out rather than relying on serialized RectTransform values,
// so it stays put in the bottom corner whatever the aspect ratio, and wires its own
// onClick instead of needing an inspector-assigned event.
[ExecuteAlways]
[RequireComponent(typeof(Button))]
public class VideoSkipButton : MonoBehaviour
{
    [SerializeField] Vector2 size = new Vector2(150f, 160f);
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

        // bottom-right corner of the video image
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 0f);
        rt.sizeDelta = size;
        rt.anchoredPosition = new Vector2(-margin.x, margin.y);
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
    }

    void Skip()
    {
        if (RandomButtonGenerator.Instance != null)
            RandomButtonGenerator.Instance.SkipVideo();
    }
}
