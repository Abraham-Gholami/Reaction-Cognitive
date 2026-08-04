using UnityEngine;

// Adjusts an orthographic camera's size so the assigned world-space background
// sprite always COVERS the screen (no gaps) across every aspect ratio — tall
// phones through tablets. Updates live on resolution/orientation changes.
// Runtime only: running in edit mode let a degenerate Game-view aspect ratio compute a
// nonsense size and serialize it into the scene, destroying the authored framing.
[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class CameraBackgroundFit : MonoBehaviour
{
    [Tooltip("The world-space background sprite that must always fill the screen.")]
    public SpriteRenderer background;

    [Tooltip("Framing to keep while the background already covers the screen. 0 = adopt the camera's authored size.")]
    public float maxOrthographicSize;

    Camera cam;
    int lastW, lastH;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        if (maxOrthographicSize <= 0f && cam != null) maxOrthographicSize = cam.orthographicSize;
        Apply();
    }

    void Update()
    {
        if (Screen.width != lastW || Screen.height != lastH)
            Apply();
    }

    void Apply()
    {
        if (cam == null) cam = GetComponent<Camera>();
        lastW = Screen.width;
        lastH = Screen.height;

        if (!cam.orthographic || background == null || background.sprite == null) return;
        if (lastW <= 0 || lastH <= 0) return;

        // Refuse to act on an implausible viewport (collapsed or mid-resize), which is
        // what produced the corrupt size that got saved into the scene.
        float aspect = (float)lastW / lastH;
        if (aspect < 0.2f || aspect > 5f) return;

        Vector3 scale = background.transform.lossyScale;
        Vector2 size = background.sprite.bounds.size;
        float spriteW = Mathf.Abs(size.x * scale.x);
        float spriteH = Mathf.Abs(size.y * scale.y);
        if (spriteW <= 0f || spriteH <= 0f) return;

        // Cover: largest ortho size where the sprite still fills BOTH dimensions.
        float orthoForHeight = spriteH * 0.5f;
        float orthoForWidth = spriteW / (2f * aspect);
        float cover = Mathf.Min(orthoForHeight, orthoForWidth);

        // Never zoom out past the authored framing — only in, and only as far as the
        // aspect ratio demands. On the design aspect this leaves the camera untouched.
        cam.orthographicSize = maxOrthographicSize > 0f ? Mathf.Min(maxOrthographicSize, cover) : cover;
    }
}
