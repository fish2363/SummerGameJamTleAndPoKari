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
    [SerializeField] private SoundID deadSound;
    [SerializeField] private CanvasGroup fakeHeart;
    
    [Header("애니메이션 시간")]
    public float duration = 0.5f;
    
    [Header("리더보드 설정")]
    [SerializeField] private bool useLeaderboard = true; // 리더보드 사용 여부
    [SerializeField] private string gameOverRankingPanelId = "gameOverRanking"; // PanelManager에서 찾을 패널 ID
    [SerializeField] private bool initializeServicesIfNeeded = true; // 필요시 서비스 자동 초기화

    // 하이스코어 키 상수 (GameOverRankingMenu와 일치)
    private const string HIGH_SCORE_KEY = "HighScore";

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
        
        // 게임 시작 시 Unity Services 상태 확인 및 초기화 (선택사항)
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
    /// 필요시 Unity Services 초기화 (백그라운드에서)
    /// </summary>
    private IEnumerator InitializeServicesIfNeeded()
    {
        // 이미 초기화되어 있으면 스킵
        if (UnityServices.State == ServicesInitializationState.Initialized && 
            AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Unity Services 이미 초기화됨");
            yield break;
        }

        Debug.Log("백그라운드에서 Unity Services 초기화 시도...");

        // Unity Services 초기화
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
                    Debug.Log("Unity Services 백그라운드 초기화 성공");
                }
                else
                {
                    Debug.LogWarning($"Unity Services 초기화 실패: {task.Exception?.Message}");
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
                Debug.LogWarning("Unity Services 초기화 실패 또는 타임아웃");
                yield break;
            }
        }

        // 익명 로그인
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
                    Debug.Log("백그라운드 익명 로그인 성공");
                    
                    // 저장된 플레이어 이름 설정
                    string savedName = PlayerPrefs.GetString("SavedPlayerName", "Player");
                    AuthenticationService.Instance.UpdatePlayerNameAsync(savedName);
                }
                else
                {
                    Debug.LogWarning($"익명 로그인 실패: {task.Exception?.Message}");
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
                Debug.LogWarning("익명 로그인 실패 또는 타임아웃 (게임플레이에는 영향 없음)");
            }
        }
    }

    public void DeadEvent()
    {
        try
        {
            // 현재 게임 시간 가져오기 (안전하게)
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
                Debug.LogWarning("TimeManager.Instance가 null입니다.");
                if (timeText != null)
                    timeText.text = "0.00";
            }
            
            // 하이스코어 설정 (안전하게)
            SetHighScore();
            
            // 리더보드에 점수 업로드 (기존 게임오버 애니메이션 전에)
            if (useLeaderboard)
            {
                UploadScoreToLeaderboard();
            }
            
            // 기존 게임오버 애니메이션 실행
            StartCoroutine(DeadRoutine());
        }
        catch (Exception e)
        {
            Debug.LogError($"DeadEvent 실행 중 오류: {e.Message}");
            
            // 오류 발생 시에도 최소한 게임오버 애니메이션은 실행
            try
            {
                StartCoroutine(DeadRoutine());
            }
            catch (Exception fallbackError)
            {
                Debug.LogError($"DeadRoutine 실행 중 추가 오류: {fallbackError.Message}");
            }
        }
    }

    /// <summary>
    /// 리더보드에 점수 업로드 (개선된 버전)
    /// </summary>
    private void UploadScoreToLeaderboard()
    {
        try
        {
            // 현재 점수와 시간 가져오기 (안전하게)
            int currentScore = 0;
            float playTime = 0f;
            
            if (ScoreManager.Instance != null)
            {
                currentScore = ScoreManager.Instance.CurrentScore;
            }
            else
            {
                Debug.LogWarning("ScoreManager.Instance가 null입니다. 점수를 0으로 설정합니다.");
            }
            
            if (TimeManager.Instance != null)
            {
                playTime = TimeManager.Instance.CurrentTime;
            }
            else
            {
                Debug.LogWarning("TimeManager.Instance가 null입니다. 시간을 0으로 설정합니다.");
            }
            
            Debug.Log($"리더보드 업로드 시도: 점수={currentScore}, 시간={playTime:F2}초");
            
            // Unity Services 상태 확인
            if (!IsServicesReady())
            {
                Debug.LogWarning("Unity Services가 준비되지 않음. 로컬 점수만 저장하고 오프라인 모드로 진행합니다.");
                SafeHandleOfflineMode(currentScore, playTime);
                return;
            }
            
            // GameOverRankingMenu 찾기 (PanelManager 사용)
            try
            {
                GameOverRankingMenu rankingMenu = (GameOverRankingMenu)PanelManager.GetSingleton(gameOverRankingPanelId);
                
                if (rankingMenu != null)
                {
                    // 리더보드에 점수 업로드 및 랭킹 표시
                    rankingMenu.ShowRankingAfterScore(currentScore, playTime);
                    Debug.Log("리더보드 업로드 요청 전송!");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"PanelManager에서 GameOverRankingMenu 찾기 실패: {e.Message}");
            }
            
            // 대안: FindObjectOfType으로 찾기
            try
            {
                GameOverRankingMenu fallbackMenu = FindObjectOfType<GameOverRankingMenu>();
                if (fallbackMenu != null)
                {
                    fallbackMenu.ShowRankingAfterScore(currentScore, playTime);
                    Debug.Log("대안 방법으로 리더보드 업로드 요청 전송!");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"FindObjectOfType으로 GameOverRankingMenu 찾기 실패: {e.Message}");
            }
            
            // 모든 시도 실패 시 오프라인 모드로 진행
            Debug.LogError("GameOverRankingMenu를 전혀 찾을 수 없습니다. 로컬 저장만 진행합니다.");
            SafeHandleOfflineMode(currentScore, playTime);
        }
        catch (Exception e)
        {
            Debug.LogError($"UploadScoreToLeaderboard 실행 중 오류: {e.Message}");
            
            // 오류 발생 시 기본값으로 오프라인 모드 처리
            SafeHandleOfflineMode(0, 0f);
        }
    }

    /// <summary>
    /// 안전한 오프라인 모드 처리
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
            Debug.LogError($"오프라인 모드 처리 중 오류: {e.Message}");
        }
    }

    /// <summary>
    /// Unity Services 준비 상태 확인
    /// </summary>
    private bool IsServicesReady()
    {
        return UnityServices.State == ServicesInitializationState.Initialized && 
               AuthenticationService.Instance.IsSignedIn;
    }

    /// <summary>
    /// 로컬에 점수 저장
    /// </summary>
    private void SaveScoreLocally(int score, float playTime)
    {
        try
        {
            // 하이스코어 확인 및 저장
            int currentHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            if (score > currentHighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
                PlayerPrefs.SetFloat("BestTime", playTime);
                PlayerPrefs.Save();
                Debug.Log($"새로운 하이스코어 로컬 저장: {score}점");
            }
            
            // 게임 플레이 횟수 증가
            int totalGames = PlayerPrefs.GetInt("TotalGames", 0);
            PlayerPrefs.SetInt("TotalGames", totalGames + 1);
            PlayerPrefs.Save();
            
            Debug.Log($"점수 로컬 저장 완료: {score}점, {playTime:F2}초");
        }
        catch (Exception e)
        {
            Debug.LogError($"로컬 점수 저장 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 오프라인 게임오버 표시 (리더보드 없이)
    /// </summary>
    private void ShowOfflineGameOver()
    {
        Debug.Log("오프라인 모드로 게임오버 화면 표시");
        // 필요하다면 여기에 오프라인 전용 게임오버 UI 표시 로직 추가
        // 예: 간단한 팝업이나 메시지 표시
    }

    public void SetHighScore()
    {
        try
        {
            int levelNum = 0;
            
            // ScoreManager 안전하게 접근
            if (ScoreManager.Instance != null)
            {
                levelNum = ScoreManager.Instance.CurrentScore;
            }
            else
            {
                Debug.LogWarning("ScoreManager.Instance가 null입니다. 점수를 0으로 설정합니다.");
            }
            
            // scoreText 안전하게 설정
            if (scoreText != null)
            {
                scoreText.text = $"{levelNum}";
            }
            
            // 기존 PlayerPrefs 키와 새로운 HIGH_SCORE_KEY 모두 업데이트
            int oldHighScore = PlayerPrefs.GetInt("score", 0);
            int newHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            int displayHighScore = Mathf.Max(oldHighScore, newHighScore, levelNum);
            
            // highScoreText 안전하게 설정
            if (highScoreText != null)
            {
                highScoreText.text = $"{displayHighScore}";
            }

            // 새로운 하이스코어인 경우 두 키 모두 업데이트
            if (levelNum > displayHighScore)
            {
                PlayerPrefs.SetInt("score", levelNum);
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, levelNum);
                PlayerPrefs.Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SetHighScore 실행 중 오류: {e.Message}");
            
            // 오류 발생 시 기본값으로 텍스트 설정
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
    /// 에디터에서 테스트용
    /// </summary>
    [ContextMenu("서비스 상태 확인")]
    public void CheckServicesStatus()
    {
        Debug.Log($"Unity Services 상태: {UnityServices.State}");
        Debug.Log($"인증 상태: {(AuthenticationService.Instance.IsSignedIn ? "로그인됨" : "로그인 안됨")}");
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log($"플레이어 이름: {AuthenticationService.Instance.PlayerName}");
        }
    }
    
    [ContextMenu("강제 리더보드 테스트")]
    public void TestLeaderboard()
    {
        UploadScoreToLeaderboard();
    }
    #endif
}