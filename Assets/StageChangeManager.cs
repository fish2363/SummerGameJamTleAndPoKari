using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using Ami.BroAudio;
using Member.KYH;
using UnityEngine.SceneManagement;
using TMPro;
using Member.KYH;
using Leaderboard.Scripts.Menu;
using Leaderboard.Scripts.Tools;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class StageChangeManager : MonoBehaviour
{
    public static StageChangeManager Instance;

    public Image leftPanel;
    public Image rightPanel;

    public RectTransform leftPos;   // ������ �� ��ǥ ��ġ
    public RectTransform rightPos;

    [SerializeField] private Collider2D[] smallColli;
    [SerializeField] private Collider2D[] bigColli;
    [SerializeField] private Image[] gameOverPanel;
    [SerializeField] private CanvasGroup gameUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    private Vector2 leftOriginalPos;   // ���� ��ġ (���� ����)
    private Vector2 rightOriginalPos;

    [SerializeField] private Image gameOverUI;

    [SerializeField] private SoundID inGameMusic;
    [SerializeField] private SoundID deadSound;
    [SerializeField] private CanvasGroup fakeHeart;
    
    [Header("�ִϸ��̼� �ð�")]
    public float duration = 0.5f;
    
    [Header("�������� ����")]
    [SerializeField] private bool useLeaderboard = true; // �������� ��� ����
    [SerializeField] private string gameOverRankingPanelId = "gameOverRanking"; // PanelManager���� ã�� �г� ID
    [SerializeField] private bool initializeServicesIfNeeded = true; // �ʿ�� ���� �ڵ� �ʱ�ȭ

    // ���̽��ھ� Ű ��� (GameOverRankingMenu�� ��ġ)
    private const string HIGH_SCORE_KEY = "HighScore";

    private void Awake()
    {
        Instance = this;

        // ���� anchoredPosition ���� (���� ����)
        leftOriginalPos = leftPanel.rectTransform.anchoredPosition;
        rightOriginalPos = rightPanel.rectTransform.anchoredPosition;
    }

    private void Start()
    {
        inGameMusic.Play();
        
        // ���� ���� �� Unity Services ���� Ȯ�� �� �ʱ�ȭ (���û���)
        if (useLeaderboard && initializeServicesIfNeeded)
        {
            StartCoroutine(InitializeServicesIfNeeded());
        }
    }

    private void OnDestroy()
    {
        BroAudio.Stop(inGameMusic);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) OpenPanels();
        if (Input.GetKeyDown(KeyCode.O)) ClosePanels();
    }

    /// <summary>
    /// �ʿ�� Unity Services �ʱ�ȭ (��׶��忡��)
    /// </summary>
    private IEnumerator InitializeServicesIfNeeded()
    {
        // �̹� �ʱ�ȭ�Ǿ� ������ ��ŵ
        if (UnityServices.State == ServicesInitializationState.Initialized && 
            AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Unity Services �̹� �ʱ�ȭ��");
            yield break;
        }

        Debug.Log("��׶��忡�� Unity Services �ʱ�ȭ �õ�...");

        // Unity Services �ʱ�ȭ
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            bool initComplete = false;
            bool initSuccess = false;
            
            UnityServices.InitializeAsync().ContinueWith(task =>
            {
                initComplete = true;
                initSuccess = !task.IsFaulted;
                if (initSuccess)
                {
                    Debug.Log("Unity Services ��׶��� �ʱ�ȭ ����");
                }
                else
                {
                    Debug.LogWarning($"Unity Services �ʱ�ȭ ����: {task.Exception?.Message}");
                }
            });

            float timeout = 10f;
            while (!initComplete && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            if (!initSuccess || timeout <= 0)
            {
                Debug.LogWarning("Unity Services �ʱ�ȭ ���� �Ǵ� Ÿ�Ӿƿ�");
                yield break;
            }
        }

        // �͸� �α���
        if (UnityServices.State == ServicesInitializationState.Initialized && 
            !AuthenticationService.Instance.IsSignedIn)
        {
            bool signInComplete = false;
            bool signInSuccess = false;
            
            AuthenticationService.Instance.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                signInComplete = true;
                signInSuccess = !task.IsFaulted;
                if (signInSuccess)
                {
                    Debug.Log("��׶��� �͸� �α��� ����");
                    
                    // ����� �÷��̾� �̸� ����
                    string savedName = PlayerPrefs.GetString("SavedPlayerName", "Player");
                    AuthenticationService.Instance.UpdatePlayerNameAsync(savedName);
                }
                else
                {
                    Debug.LogWarning($"�͸� �α��� ����: {task.Exception?.Message}");
                }
            });

            float timeout = 10f;
            while (!signInComplete && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            if (!signInSuccess || timeout <= 0)
            {
                Debug.LogWarning("�͸� �α��� ���� �Ǵ� Ÿ�Ӿƿ� (�����÷��̿��� ���� ����)");
            }
        }
    }

    public void DeadEvent()
    {
        try
        {
            // ���� ���� �ð� �������� (�����ϰ�)
            float currentTime = 0f;
            if (TimeManager.Instance != null)
            {
                currentTime = TimeManager.Instance.CurrentTime;
                if (timeText != null)
                {
                    string timeString = currentTime.ToString();
                    timeText.text = timeString.Length > 6 ? timeString.Substring(0, 6) : timeString;
                }
            }
            else
            {
                Debug.LogWarning("TimeManager.Instance�� null�Դϴ�.");
                if (timeText != null)
                    timeText.text = "0.00";
            }
            
            // ���̽��ھ� ���� (�����ϰ�)
            SetHighScore();
            
            // �������忡 ���� ���ε� (���� ���ӿ��� �ִϸ��̼� ����)
            if (useLeaderboard)
            {
                UploadScoreToLeaderboard();
            }
            
            // ���� ���ӿ��� �ִϸ��̼� ����
            StartCoroutine(DeadRoutine());
        }
        catch (Exception e)
        {
            Debug.LogError($"DeadEvent ���� �� ����: {e.Message}");
            
            // ���� �߻� �ÿ��� �ּ��� ���ӿ��� �ִϸ��̼��� ����
            try
            {
                StartCoroutine(DeadRoutine());
            }
            catch (Exception fallbackError)
            {
                Debug.LogError($"DeadRoutine ���� �� �߰� ����: {fallbackError.Message}");
            }
        }
    }

    /// <summary>
    /// �������忡 ���� ���ε� (������ ����)
    /// </summary>
    private void UploadScoreToLeaderboard()
    {
        try
        {
            // ���� ������ �ð� �������� (�����ϰ�)
            int currentScore = 0;
            float playTime = 0f;
            
            if (ScoreManager.Instance != null)
            {
                currentScore = ScoreManager.Instance.CurrentScore;
            }
            else
            {
                Debug.LogWarning("ScoreManager.Instance�� null�Դϴ�. ������ 0���� �����մϴ�.");
            }
            
            if (TimeManager.Instance != null)
            {
                playTime = TimeManager.Instance.CurrentTime;
            }
            else
            {
                Debug.LogWarning("TimeManager.Instance�� null�Դϴ�. �ð��� 0���� �����մϴ�.");
            }
            
            Debug.Log($"�������� ���ε� �õ�: ����={currentScore}, �ð�={playTime:F2}��");
            
            // Unity Services ���� Ȯ��
            if (!IsServicesReady())
            {
                Debug.LogWarning("Unity Services�� �غ���� ����. ���� ������ �����ϰ� �������� ���� �����մϴ�.");
                SafeHandleOfflineMode(currentScore, playTime);
                return;
            }
            
            // GameOverRankingMenu ã�� (PanelManager ���)
            try
            {
                GameOverRankingMenu rankingMenu = (GameOverRankingMenu)PanelManager.GetSingleton(gameOverRankingPanelId);
                
                if (rankingMenu != null)
                {
                    // �������忡 ���� ���ε� �� ��ŷ ǥ��
                    rankingMenu.ShowRankingAfterScore(currentScore, playTime);
                    Debug.Log("�������� ���ε� ��û ����!");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"PanelManager���� GameOverRankingMenu ã�� ����: {e.Message}");
            }
            
            // ���: FindObjectOfType���� ã��
            try
            {
                GameOverRankingMenu fallbackMenu = FindObjectOfType<GameOverRankingMenu>();
                if (fallbackMenu != null)
                {
                    fallbackMenu.ShowRankingAfterScore(currentScore, playTime);
                    Debug.Log("��� ������� �������� ���ε� ��û ����!");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"FindObjectOfType���� GameOverRankingMenu ã�� ����: {e.Message}");
            }
            
            // ��� �õ� ���� �� �������� ���� ����
            Debug.LogError("GameOverRankingMenu�� ���� ã�� �� �����ϴ�. ���� ���常 �����մϴ�.");
            SafeHandleOfflineMode(currentScore, playTime);
        }
        catch (Exception e)
        {
            Debug.LogError($"UploadScoreToLeaderboard ���� �� ����: {e.Message}");
            
            // ���� �߻� �� �⺻������ �������� ��� ó��
            SafeHandleOfflineMode(0, 0f);
        }
    }

    /// <summary>
    /// ������ �������� ��� ó��
    /// </summary>
    private void SafeHandleOfflineMode(int currentScore, float playTime)
    {
        try
        {
            SaveScoreLocally(currentScore, playTime);
            ShowOfflineGameOver();
        }
        catch (Exception e)
        {
            Debug.LogError($"�������� ��� ó�� �� ����: {e.Message}");
        }
    }

    /// <summary>
    /// Unity Services �غ� ���� Ȯ��
    /// </summary>
    private bool IsServicesReady()
    {
        return UnityServices.State == ServicesInitializationState.Initialized && 
               AuthenticationService.Instance.IsSignedIn;
    }

    /// <summary>
    /// ���ÿ� ���� ����
    /// </summary>
    private void SaveScoreLocally(int score, float playTime)
    {
        try
        {
            // ���̽��ھ� Ȯ�� �� ����
            int currentHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            if (score > currentHighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
                PlayerPrefs.SetFloat("BestTime", playTime);
                PlayerPrefs.Save();
                Debug.Log($"���ο� ���̽��ھ� ���� ����: {score}��");
            }
            
            // ���� �÷��� Ƚ�� ����
            int totalGames = PlayerPrefs.GetInt("TotalGames", 0);
            PlayerPrefs.SetInt("TotalGames", totalGames + 1);
            PlayerPrefs.Save();
            
            Debug.Log($"���� ���� ���� �Ϸ�: {score}��, {playTime:F2}��");
        }
        catch (Exception e)
        {
            Debug.LogError($"���� ���� ���� ����: {e.Message}");
        }
    }

    /// <summary>
    /// �������� ���ӿ��� ǥ�� (�������� ����)
    /// </summary>
    private void ShowOfflineGameOver()
    {
        Debug.Log("�������� ���� ���ӿ��� ȭ�� ǥ��");
        // �ʿ��ϴٸ� ���⿡ �������� ���� ���ӿ��� UI ǥ�� ���� �߰�
        // ��: ������ �˾��̳� �޽��� ǥ��
    }

    public void SetHighScore()
    {
        try
        {
            int levelNum = 0;
            
            // ScoreManager �����ϰ� ����
            if (ScoreManager.Instance != null)
            {
                levelNum = ScoreManager.Instance.CurrentScore;
            }
            else
            {
                Debug.LogWarning("ScoreManager.Instance�� null�Դϴ�. ������ 0���� �����մϴ�.");
            }
            
            // scoreText �����ϰ� ����
            if (scoreText != null)
            {
                scoreText.text = $"{levelNum}";
            }
            
            // ���� PlayerPrefs Ű�� ���ο� HIGH_SCORE_KEY ��� ������Ʈ
            int oldHighScore = PlayerPrefs.GetInt("score", 0);
            int newHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            int displayHighScore = Mathf.Max(oldHighScore, newHighScore, levelNum);
            
            // highScoreText �����ϰ� ����
            if (highScoreText != null)
            {
                highScoreText.text = $"{displayHighScore}";
            }

            // ���ο� ���̽��ھ��� ��� �� Ű ��� ������Ʈ
            if (levelNum > displayHighScore)
            {
                PlayerPrefs.SetInt("score", levelNum);
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, levelNum);
                PlayerPrefs.Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SetHighScore ���� �� ����: {e.Message}");
            
            // ���� �߻� �� �⺻������ �ؽ�Ʈ ����
            if (scoreText != null)
                scoreText.text = "0";
            if (highScoreText != null)
                highScoreText.text = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0).ToString();
        }
    }

    private IEnumerator DeadRoutine()
    {
        Time.timeScale = 0f;
        ApiManager.Instance.ShakeScreen();
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        deadSound.Play();
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
        fakeHeart.DOFade(1f,0.2f);
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
        fakeHeart.DOFade(0f, 0.2f);
        ApiManager.Instance.IsBoss = false;
        for (int i = 0; i < 2; i++)
        {
          smallColli[i].gameObject.SetActive(true);
            bigColli[i].gameObject.SetActive(false);
        }
        leftPanel.rectTransform.DOAnchorPos(leftOriginalPos, duration).SetEase(Ease.InOutQuad);
        rightPanel.rectTransform.DOAnchorPos(rightOriginalPos, duration).SetEase(Ease.InOutQuad);
    }

    #if UNITY_EDITOR
    /// <summary>
    /// �����Ϳ��� �׽�Ʈ��
    /// </summary>
    [ContextMenu("���� ���� Ȯ��")]
    public void CheckServicesStatus()
    {
        Debug.Log($"Unity Services ����: {UnityServices.State}");
        Debug.Log($"���� ����: {(AuthenticationService.Instance.IsSignedIn ? "�α��ε�" : "�α��� �ȵ�")}");
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log($"�÷��̾� �̸�: {AuthenticationService.Instance.PlayerName}");
        }
    }
    
    [ContextMenu("���� �������� �׽�Ʈ")]
    public void TestLeaderboard()
    {
        UploadScoreToLeaderboard();
    }
    #endif
}