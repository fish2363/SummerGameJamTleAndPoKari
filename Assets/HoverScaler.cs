using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScaler : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public float scaleUpSize = 1.1f;
    public float duration = 0.2f;

    private Vector3 originalScale;
    private Tween scaleTween;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale * scaleUpSize, duration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }
}
