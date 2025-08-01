using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageChangeManager : MonoBehaviour
{
    public static StageChangeManager Instance;

    public Image leftPanel;
    public Image rightPanel;

    public RectTransform leftPos;   // 열렸을 때 목표 위치
    public RectTransform rightPos;

    [SerializeField] private Collider2D[] smallColli;
    [SerializeField] private Collider2D[] bigColli;

    private Vector2 leftOriginalPos;   // 시작 위치 (닫힌 상태)
    private Vector2 rightOriginalPos;

    [Header("애니메이션 시간")]
    public float duration = 0.5f;

    private void Awake()
    {
        Instance = this;

        // 현재 anchoredPosition 저장 (닫힌 상태)
        leftOriginalPos = leftPanel.rectTransform.anchoredPosition;
        rightOriginalPos = rightPanel.rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) OpenPanels();
        if (Input.GetKeyDown(KeyCode.O)) ClosePanels();
    }

    public void OpenPanels()
    {
        ApiManager.Instance.IsBoss = true;
        for(int i=0;i<2;i++)
        {
            smallColli[i].enabled = false;
            bigColli[i].enabled = true;
        }
        leftPanel.rectTransform.DOAnchorPos(leftPos.anchoredPosition, duration).SetEase(Ease.InOutQuad);
        rightPanel.rectTransform.DOAnchorPos(rightPos.anchoredPosition, duration).SetEase(Ease.InOutQuad);
    }

    public void ClosePanels()
    {
        ApiManager.Instance.IsBoss = false;
        for (int i = 0; i < 2; i++)
        {
            smallColli[i].enabled = true;
            bigColli[i].enabled = false;
        }
        leftPanel.rectTransform.DOAnchorPos(leftOriginalPos, duration).SetEase(Ease.InOutQuad);
        rightPanel.rectTransform.DOAnchorPos(rightOriginalPos, duration).SetEase(Ease.InOutQuad);
    }
}