using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboCount : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text percentText;
    public TMP_Text ComboText;

    private Transform tr;

    void Awake()
    {
        tr = transform;
    }

    public void SetText(string value, string comboPercent)
    {
        text.text = value;
        percentText.text = comboPercent;
    }

    public void Animate(int comboCount)
    {
        float baseScale = 0.3216014f;
        float scaleFactor = Mathf.Clamp(1f + comboCount * 0.1f, 1f, 2f);
        float targetScale = baseScale * scaleFactor;

        Vector3 currentPos = transform.position;

        // 상승 거리 계산
        float mapTopY = 4.82f;
        float maxY = mapTopY;
        float maxRise = maxY - currentPos.y;

        // 콤보 수에 따라 상승량, 단 맵 밖으로는 안 나가게 제한
        float desiredRise = 1f + comboCount * 0.2f; // 콤보 수가 많아질수록 조금씩 더 뜨게
        float rise = Mathf.Clamp(desiredRise, 0f, maxRise);

        // 텍스트 색상 변화
        if (comboCount > 20) text.color = Color.red;
        else if (comboCount > 10) text.color = new Color(1f, 0.5f, 0f);
        else text.color = Color.white;

        // 초기 스케일
        transform.localScale = Vector3.one * baseScale * 0.1f;

        // 팡! 크기 변화
        transform.DOScale(targetScale, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOScale(baseScale, 0.1f));

        // 위로 떠오르기
        transform.DOMoveY(currentPos.y + rise, 1f)
            .SetEase(Ease.OutCubic);

        // 회전 효과
        float rotZ = Random.Range(-10f, 10f);
        transform.DOLocalRotate(new Vector3(0, 0, rotZ), 0.3f);

        // 페이드아웃
        text.DOFade(0f, 0.5f).SetDelay(0.5f);
        percentText.DOFade(0f, 0.5f).SetDelay(0.5f);
        ComboText.DOFade(0f, 0.5f).SetDelay(0.5f);

        Destroy(gameObject, 2f);
    }
}