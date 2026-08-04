using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// Reconstructed (was never committed). Small UI press feedback: scales the
// button down on press and back on release. Has no serialized fields, matching
// the scene components that reference it.
public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    const float PressedScale = 0.92f;
    const float Duration = 0.08f;

    Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnDisable()
    {
        transform.DOKill();
        transform.localScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale * PressedScale, Duration).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, Duration).SetUpdate(true);
    }
}
