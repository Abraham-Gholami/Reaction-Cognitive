using UnityEngine;

// Resizes this RectTransform to the device's safe area (avoids notches, rounded
// corners and the home indicator). Put HUD / menu content under a stretched
// child that carries this component. Updates live on orientation/resolution change.
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
[DisallowMultipleComponent]
public class SafeArea : MonoBehaviour
{
    RectTransform rt;
    Rect lastSafe;
    Vector2Int lastScreen;

    void OnEnable()
    {
        rt = GetComponent<RectTransform>();
        Apply();
    }

    void Update()
    {
        if (Screen.safeArea != lastSafe ||
            Screen.width != lastScreen.x || Screen.height != lastScreen.y)
            Apply();
    }

    void Apply()
    {
        if (rt == null) rt = GetComponent<RectTransform>();
        if (Screen.width <= 0 || Screen.height <= 0) return;

        Rect safe = Screen.safeArea;
        lastSafe = safe;
        lastScreen = new Vector2Int(Screen.width, Screen.height);

        Vector2 anchorMin = safe.position;
        Vector2 anchorMax = safe.position + safe.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        if (float.IsNaN(anchorMin.x) || float.IsNaN(anchorMax.x)) return;

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
