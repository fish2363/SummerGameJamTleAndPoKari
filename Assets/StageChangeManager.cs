using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using Ami.BroAudio;
using Member.KYH;
using UnityEngine.SceneManagement;
using TMPro;

public class StageChangeManager : MonoBehaviour
{
    public static StageChangeManager Instance;

    public Image leftPanel;
    public Image rightPanel;

    public RectTransform leftPos;   // 열렸을 때 목표 위치
    public RectTransform rightPos;

    [SerializeField] private Collider2D[] smallColli;
    [SerializeField] private Collider2D[] bigColli;
    [SerializeField] private Image[] gameOverPanel;
    [SerializeField] private CanvasGroup gameUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    private Vector2 leftOriginalPos;   // 시작 위치 (닫힌 상태)
    private Vector2 rightOriginalPos;

    [SerializeField] private Image gameOverUI;

    [SerializeField] private SoundID inGameMusic;

    [Header("애니메이션 시간")]
    public float duration = 0.5f;

    private void Awake()
    {
        Instance = this;

        // 현재 anchoredPosition 저장 (닫힌 상태)
        leftOriginalPos = leftPanel.rectTransform.anchoredPosition;
        rightOriginalPos = rightPanel.rectTransform.anchoredPosition;
    }

    private void Start()
    {
        inGameMusic.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) OpenPanels();
        if (Input.GetKeyDown(KeyCode.O)) ClosePanels();
    }

    public void DeadEvent()
    {
        timeText.text = $"{TimeManager.Instance.CurrentTime}".Substring(0,6);
        SetHighScore();
        StartCoroutine(DeadRoutine());
    }
    public void SetHighScore()
    {
        int levelNum = ScoreManager.Instance.CurrentScore;
        scoreText.text = $"{levelNum}";
        highScoreText.text = $"{PlayerPrefs.GetInt("score", levelNum)}";

        if (levelNum > PlayerPrefs.GetInt("score", 0))
            PlayerPrefs.SetInt("score", levelNum);
    }

    private IEnumerator DeadRoutine()
    {
        Time.timeScale = 0f;
        ApiManager.Instance.ShakeScreen();
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        for (int i = 0; i < gameOverPanel.Length; i++)
            gameOverPanel[i].rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.InOutQuad).WaitForCompletion();
        gameUI.DOFade(0f, 0.5f);
        gameOverUI.rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.InOutQuad);
    }

    public void SceneChanger(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void OpenPanels()
    {
        ApiManager.Instance.IsBoss = true;
        for(int i=0;i<2;i++)
        {
          smallColli[i].gameObject.SetActive(false);
            bigColli[i].gameObject.SetActive(true);
        }
        leftPanel.rectTransform.DOAnchorPos(leftPos.anchoredPosition, duration).SetEase(Ease.InOutQuad);
        rightPanel.rectTransform.DOAnchorPos(rightPos.anchoredPosition, duration).SetEase(Ease.InOutQuad);
    }

    public void ClosePanels()
    {
        ApiManager.Instance.IsBoss = false;
        for (int i = 0; i < 2; i++)
        {
          smallColli[i].gameObject.SetActive(true);
            bigColli[i].gameObject.SetActive(false);
        }
        leftPanel.rectTransform.DOAnchorPos(leftOriginalPos, duration).SetEase(Ease.InOutQuad);
        rightPanel.rectTransform.DOAnchorPos(rightOriginalPos, duration).SetEase(Ease.InOutQuad);
    }
}