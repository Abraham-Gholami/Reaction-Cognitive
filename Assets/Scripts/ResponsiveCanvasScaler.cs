using UnityEngine;
using UnityEngine.UI;

// Keeps a CanvasScaler's match value set so the ENTIRE reference resolution
// always fits on screen (expand-to-fit, never crops gameplay). Background art
// fills any extra space. Works across all portrait aspect ratios (tall phones
// through tablets) and updates live if the resolution/orientation changes.
[RequireComponent(typeof(CanvasScaler))]
[ExecuteAlways]
[DisallowMultipleComponent]
public class ResponsiveCanvasScaler : MonoBehaviour
{
    CanvasScaler scaler;
    int lastW, lastH;

    void OnEnable()
    {
        scaler = GetComponent<CanvasScaler>();
        Apply();
    }

    void Update()
    {
        if (Screen.width != lastW || Screen.height != lastH)
            Apply();
    }

    void Apply()
    {
        if (scaler == null) scaler = GetComponent<CanvasScaler>();
        lastW = Screen.width;
        lastH = Screen.height;

        if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            return;

        Vector2 refRes = scaler.referenceResolution;
        if (refRes.x <= 0f || refRes.y <= 0f || lastW <= 0 || lastH <= 0)
            return;

        float refAspect = refRes.x / refRes.y;
        float screenAspect = (float)lastW / lastH;

        // Narrower/taller than the design -> match width (fit full design width).
        // Wider than the design -> match height (fit full design height).
        // Either way the whole design area stays visible.
        scaler.matchWidthOrHeight = (screenAspect < refAspect) ? 0f : 1f;
    }
}
