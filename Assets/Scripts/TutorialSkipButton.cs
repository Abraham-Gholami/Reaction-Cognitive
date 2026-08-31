using UnityEngine;
using UnityEngine.UI;

// Editor-only skip for the tutorial / voice-over panels, the counterpart to
// VideoSkipButton. The same reasoning applies: icon-only, because a text label here
// once rendered nothing at all (digits-only font atlas, then a stale TMP mesh), and it
// lays itself out and wires its own onClick rather than depending on serialized values.
//
// It sits under the root Canvas rather than under the tutorial panel. That panel is a
// 3670x7910 rect at 0.4 scale, so anything parented to it inherits both the scale and
// the oversized rect and cannot be pinned to a screen corner. Visibility follows the
// panel instead of the hierarchy.
//
// Visibility is toggled through the Graphic and the Button rather than SetActive, so
// Update keeps running and can turn it back on when the next panel appears.
[ExecuteAlways]
[RequireComponent(typeof(Button))]
public class TutorialSkipButton : MonoBehaviour
{
    [SerializeField] Vector2 size = new Vector2(150f, 160f);
    [SerializeField] Vector2 margin = new Vector2(60f, 80f);

    Button button;
    Graphic icon;

    void OnEnable()
    {
        Cache();
        Layout();
    }

    void Awake()
    {
        Cache();
        if (!Application.isPlaying) return;
#if UNITY_EDITOR
        button.onClick.AddListener(Skip);
        SetVisible(false);
#else
        // The tutorials are part of the test: a child must sit through them, so the
        // skip button must not exist in a build.
        gameObject.SetActive(false);
#endif
    }

    void Cache()
    {
        if (button == null) button = GetComponent<Button>();
        if (icon == null) icon = GetComponent<Graphic>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        Cache();
        Layout();
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        var generator = RandomButtonGenerator.Instance;
        SetVisible(generator != null && generator.TutorialPanelShowing);
    }
#endif

    // A disabled Graphic is removed from the raycast registry, so this hides the button
    // and stops it eating taps in one go. Button.interactable is deliberately left
    // alone - it was a second gate on the same thing and only added a way to end up
    // visible but dead.
    void SetVisible(bool visible)
    {
        Cache();
        if (icon != null) icon.enabled = visible;
    }

    void Layout()
    {
        var rt = transform as RectTransform;
        if (rt == null) return;

        // Inherit the canvas layer: an object left on Default is silently culled by a
        // camera whose mask covers only the canvas layer - present in the hierarchy,
        // drawn nowhere. This cost four rounds of debugging on the video button.
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            var layer = canvas.gameObject.layer;
            gameObject.layer = layer;
            foreach (var t in GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;
        }

        // Last sibling of the canvas, so it is both drawn over the tutorial panel and
        // first in the raycast order - uGUI hit-tests in draw order.
        rt.SetAsLastSibling();

        // bottom-right corner, matching the video skip button
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 0f);
        rt.sizeDelta = size;
        rt.anchoredPosition = new Vector2(-margin.x, margin.y);
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
    }

    void Skip()
    {
        if (RandomButtonGenerator.Instance != null)
            RandomButtonGenerator.Instance.SkipTutorial();
    }
}
