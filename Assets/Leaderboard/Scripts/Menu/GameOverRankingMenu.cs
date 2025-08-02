using System;
using System.Collections;
using System.Collections.Generic;
using Leaderboard.Scripts.Tools;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboard.Scripts.Menu
{
    public class GameOverRankingMenu : Panel
    {
        [Header("리더보드 설정")]
        [SerializeField] private string leaderboardId = "LeaderboardFish";
        
        [Header("상위 순위 UI")]
        [SerializeField] private TextMeshProUGUI upperRankText;
        [SerializeField] private TextMeshProUGUI upperNameText;
        [SerializeField] private TextMeshProUGUI upperScoreText;
        
        [Header("내 순위 UI")]
        [SerializeField] private TextMeshProUGUI myRankText;
        [SerializeField] private TextMeshProUGUI myNameText;
        [SerializeField] private TextMeshProUGUI myScoreText;
        
        [Header("하위 순위 UI")]
        [SerializeField] private TextMeshProUGUI lowerRankText;
        [SerializeField] private TextMeshProUGUI lowerNameText;
        [SerializeField] private TextMeshProUGUI lowerScoreText;
        
        [Header("현재 게임 정보 UI")]
        [SerializeField] private TextMeshProUGUI currentScoreText;
        [SerializeField] private TextMeshProUGUI currentTimeText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private GameObject newRecordIndicator;
        
        [Header("버튼")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button offlineRetryButton; // 오프라인 모드용 재시도 버튼
        
        [Header("오프라인 모드 UI (선택사항)")]
        [SerializeField] private GameObject onlineRankingPanel; // 온라인 랭킹 패널
        [SerializeField] private GameObject offlineMessagePanel; // 오프라인 메시지 패널
        [SerializeField] private TextMeshProUGUI offlineMessageText; // 오프라인 메시지 텍스트

        // PlayerPrefs 키 상수
        private const string HIGH_SCORE_KEY = "HighScore";
        private const string BEST_TIME_KEY = "BestTime";
        private const string TOTAL_GAMES_KEY = "TotalGames";

        // 현재 게임 정보 저장
        private int currentGameScore = 0;
        private float currentGameTime = 0f;
        private bool isNewRecord = false;
        
        // 상태 관리
        private bool isProcessingRanking = false;
        private bool isOfflineMode = false;

        public override void Initialize()
        {
            if (IsInitialized) return;
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            
            if (retryButton != null)
                retryButton.onClick.AddListener(RetryGame);
                
            if (offlineRetryButton != null)
                offlineRetryButton.onClick.AddListener(RetryGame);
            
            if (newRecordIndicator != null)
                newRecordIndicator.SetActive(false);
            
            base.Initialize();
        }

        /// <summary>
        /// 게임 종료 후 점수와 시간으로 랭킹 표시
        /// </summary>
        public void ShowRankingAfterScore(int score, float playTime)
        {
            if (isProcessingRanking)
            {
                Debug.LogWarning("이미 랭킹 처리 중입니다.");
                return;
            }
            
            Debug.Log($"점수 업로드 및 랭킹 표시 시작: {score}점, {playTime:F2}초");
            
            // 현재 게임 정보 저장
            currentGameScore = score;
            currentGameTime = playTime;
            
            // 하이스코어 확인 및 업데이트
            CheckAndUpdateHighScore(score, playTime);
            
            // 현재 게임 정보 UI 업데이트
            UpdateCurrentGameUI();
            
            // 코루틴으로 비동기 처리
            StartCoroutine(ProcessRankingCoroutine());
        }

        /// <summary>
        /// 오프라인 모드로 게임오버 표시 (리더보드 없이)
        /// </summary>
        public void ShowOfflineGameOver(int score, float playTime, string message = "인터넷 연결을 확인해주세요.")
        {
            Debug.Log($"오프라인 모드 게임오버: {score}점, {playTime:F2}초");
            
            // 현재 게임 정보 저장
            currentGameScore = score;
            currentGameTime = playTime;
            isOfflineMode = true;
            
            // 하이스코어 확인 및 업데이트
            CheckAndUpdateHighScore(score, playTime);
            
            // 현재 게임 정보 UI 업데이트
            UpdateCurrentGameUI();
            
            // 오프라인 모드 UI 설정
            SetOfflineModeUI(message);
            
            // 게임 횟수 증가
            IncrementGameCount();
            
            // UI 표시
            Open();
        }

        /// <summary>
        /// 랭킹 처리 메인 코루틴
        /// </summary>
        private IEnumerator ProcessRankingCoroutine()
        {
            isProcessingRanking = true;
            isOfflineMode = false;
            
            // 1. 서비스 상태 확인 및 초기화
            bool servicesValid = false;
            yield return ValidateServicesCoroutine((result) => servicesValid = result);
            
            if (!servicesValid)
            {
                Debug.LogError("서비스 검증 실패 - 오프라인 모드로 전환");
                SetOfflineModeUI("서비스 연결에 문제가 있습니다. 점수는 로컬에 저장되었습니다.");
                FinishProcessing();
                yield break;
            }
            
            // 2. 점수 업로드
            LeaderboardEntry playerEntry = null;
            yield return UploadScoreCoroutine((result) => playerEntry = result);
            
            if (playerEntry == null)
            {
                Debug.LogError("점수 업로드 실패 - 오프라인 모드로 전환");
                SetOfflineModeUI("온라인 랭킹 업데이트에 실패했습니다. 점수는 로컬에 저장되었습니다.");
                FinishProcessing();
                yield break;
            }
            
            Debug.Log($"점수 업로드 성공! 내 순위: {playerEntry.Rank + 1}");
            
            // 3. 주변 순위 로드
            yield return LoadSurroundingRanksCoroutine(playerEntry.Rank);
            
            // 4. 온라인 모드 UI 설정
            SetOnlineModeUI();
            
            // 5. 게임 횟수 증가
            IncrementGameCount();
            
            // 6. 처리 완료
            FinishProcessing();
        }

        /// <summary>
        /// 처리 완료 및 UI 표시
        /// </summary>
        private void FinishProcessing()
        {
            isProcessingRanking = false;
            Open();
        }

        /// <summary>
        /// 온라인 모드 UI 설정
        /// </summary>
        private void SetOnlineModeUI()
        {
            isOfflineMode = false;
            
            if (onlineRankingPanel != null)
                onlineRankingPanel.SetActive(true);
                
            if (offlineMessagePanel != null)
                offlineMessagePanel.SetActive(false);
        }

        /// <summary>
        /// 오프라인 모드 UI 설정
        /// </summary>
        private void SetOfflineModeUI(string message)
        {
            isOfflineMode = true;
            
            if (onlineRankingPanel != null)
                onlineRankingPanel.SetActive(false);
                
            if (offlineMessagePanel != null)
                offlineMessagePanel.SetActive(true);
                
            if (offlineMessageText != null)
                offlineMessageText.text = message;
                
            // 오프라인 모드에서는 랭킹 텍스트 클리어
            ClearRankingTexts();
        }

        /// <summary>
        /// 서비스 상태 검증 및 초기화 코루틴
        /// </summary>
        private IEnumerator ValidateServicesCoroutine(System.Action<bool> callback)
        {
            // Unity Services 초기화 확인 및 시도
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.LogWarning("Unity Services가 초기화되지 않음. 초기화 시도 중...");
                
                // Unity Services 초기화 시도
                yield return InitializeUnityServicesCoroutine();
                
                // 초기화 재확인
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    Debug.LogError("Unity Services 초기화 실패");
                    callback(false);
                    yield break;
                }
            }

            // 인증 상태 확인 및 시도
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning("사용자가 로그인되지 않음. 익명 로그인 시도 중...");
                
                // 익명 로그인 시도
                yield return SignInAnonymouslyCoroutine();
                
                // 로그인 재확인
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.LogError("사용자 로그인 실패");
                    callback(false);
                    yield break;
                }
            }

            // 플레이어 이름 확인 및 설정
            string playerName = AuthenticationService.Instance.PlayerName;
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.LogWarning("플레이어 이름이 설정되지 않았습니다. 기본값 사용");
                string savedName = PlayerPrefs.GetString("SavedPlayerName", "Player");
                
                // 플레이어 이름 업데이트 시도
                yield return UpdatePlayerNameCoroutine(savedName);
            }

            Debug.Log($"서비스 검증 완료. 플레이어: {AuthenticationService.Instance.PlayerName}");
            callback(true);
        }

        /// <summary>
        /// Unity Services 초기화 코루틴
        /// </summary>
        private IEnumerator InitializeUnityServicesCoroutine()
        {
            bool initComplete = false;
            bool initSuccess = false;

            UnityServices.InitializeAsync().ContinueWith(task =>
            {
                initSuccess = !task.IsFaulted;
                initComplete = true;
                
                if (task.IsFaulted)
                {
                    Debug.LogError($"Unity Services 초기화 실패: {task.Exception?.GetBaseException()?.Message}");
                }
                else
                {
                    Debug.Log("Unity Services 초기화 성공");
                }
            });

            // 초기화 완료 대기 (최대 10초)
            float timeout = 10f;
            while (!initComplete && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            if (!initComplete)
            {
                Debug.LogError("Unity Services 초기화 타임아웃");
            }
        }

        /// <summary>
        /// 익명 로그인 코루틴
        /// </summary>
        private IEnumerator SignInAnonymouslyCoroutine()
        {
            bool signInComplete = false;
            bool signInSuccess = false;

            AuthenticationService.Instance.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                signInSuccess = !task.IsFaulted;
                signInComplete = true;
                
                if (task.IsFaulted)
                {
                    Debug.LogError($"익명 로그인 실패: {task.Exception?.GetBaseException()?.Message}");
                }
                else
                {
                    Debug.Log("익명 로그인 성공");
                    
                    // 저장된 플레이어 이름 설정
                    string savedName = PlayerPrefs.GetString("SavedPlayerName", "Player");
                    AuthenticationService.Instance.UpdatePlayerNameAsync(savedName);
                }
            });

            // 로그인 완료 대기 (최대 10초)
            float timeout = 10f;
            while (!signInComplete && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            if (!signInComplete)
            {
                Debug.LogError("익명 로그인 타임아웃");
            }
        }

        /// <summary>
        /// 플레이어 이름 업데이트 코루틴
        /// </summary>
        private IEnumerator UpdatePlayerNameCoroutine(string newName)
        {
            bool updateComplete = false;
            bool updateSuccess = false;
            
            AuthenticationService.Instance.UpdatePlayerNameAsync(newName).ContinueWith(task =>
            {
                updateSuccess = !task.IsFaulted;
                updateComplete = true;
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"플레이어 이름 업데이트 실패: {task.Exception?.GetBaseException()?.Message}");
                }
            });
            
            // 업데이트 완료 대기 (최대 5초)
            float timeout = 5f;
            while (!updateComplete && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }
            
            if (!updateComplete)
            {
                Debug.LogWarning("플레이어 이름 업데이트 타임아웃");
            }
        }

        /// <summary>
        /// 점수 업로드 코루틴
        /// </summary>
        private IEnumerator UploadScoreCoroutine(System.Action<LeaderboardEntry> callback)
        {
            const int maxRetries = 3;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                Debug.Log($"점수 업로드 시도 {attempt}/{maxRetries}");
                
                bool uploadComplete = false;
                LeaderboardEntry result = null;
                Exception uploadError = null;
                
                // 비동기 업로드 시작
                LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, currentGameScore)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            uploadError = task.Exception?.GetBaseException();
                        }
                        else
                        {
                            result = task.Result;
                        }
                        uploadComplete = true;
                    });
                
                // 업로드 완료 대기 (최대 10초)
                float timeout = 10f;
                while (!uploadComplete && timeout > 0)
                {
                    timeout -= Time.deltaTime;
                    yield return null;
                }
                
                if (!uploadComplete)
                {
                    Debug.LogError($"업로드 타임아웃 (시도 {attempt})");
                    if (attempt < maxRetries)
                    {
                        yield return new WaitForSeconds(attempt); // 점진적 대기
                        continue;
                    }
                }
                else if (uploadError != null)
                {
                    Debug.LogError($"업로드 에러 (시도 {attempt}): {uploadError.Message}");
                    
                    // 특정 에러는 재시도하지 않음
                    if (ShouldStopRetrying(uploadError))
                    {
                        Debug.LogError("재시도할 수 없는 에러입니다.");
                        break;
                    }
                    
                    if (attempt < maxRetries)
                    {
                        yield return new WaitForSeconds(attempt);
                        continue;
                    }
                }
                else
                {
                    // 성공
                    Debug.Log($"점수 업로드 성공 (시도 {attempt})");
                    callback(result);
                    yield break;
                }
            }
            
            // 모든 시도 실패
            Debug.LogError("점수 업로드 최종 실패");
            callback(null);
        }

        /// <summary>
        /// 재시도를 중단해야 하는 에러인지 확인
        /// </summary>
        private bool ShouldStopRetrying(Exception error)
        {
            if (error is Unity.Services.Leaderboards.Exceptions.LeaderboardsException leaderboardEx)
            {
                var errorCode = leaderboardEx.ErrorCode;
                return errorCode == (int)Unity.Services.Leaderboards.Exceptions.LeaderboardsExceptionReason.LeaderboardNotFound ||
                       errorCode == (int)Unity.Services.Leaderboards.Exceptions.LeaderboardsExceptionReason.Unauthorized;
            }
            return false;
        }

        /// <summary>
        /// 주변 순위 로드 코루틴
        /// </summary>
        private IEnumerator LoadSurroundingRanksCoroutine(int myRank)
        {
            List<LeaderboardEntry> allEntries = new List<LeaderboardEntry>();
            
            // 상위 순위 로드 (내 순위 - 1)
            if (myRank > 0)
            {
                yield return LoadSingleRankCoroutine(myRank - 1, allEntries);
            }
            
            // 내 순위 로드
            yield return LoadSingleRankCoroutine(myRank, allEntries);
            
            // 하위 순위 로드 (내 순위 + 1)
            yield return LoadSingleRankCoroutine(myRank + 1, allEntries);
            
            // UI 업데이트
            DisplayRankings(allEntries, myRank);
        }

        /// <summary>
        /// 단일 순위 로드 코루틴
        /// </summary>
        private IEnumerator LoadSingleRankCoroutine(int offset, List<LeaderboardEntry> resultList)
        {
            if (offset < 0) yield break; // 음수 offset은 무시
            
            bool loadComplete = false;
            List<LeaderboardEntry> entries = null;
            Exception loadError = null;
            
            var options = new GetScoresOptions
            {
                Offset = offset,
                Limit = 1
            };
            
            LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        loadError = task.Exception?.GetBaseException();
                    }
                    else if (task.Result.Results.Count > 0)
                    {
                        entries = new List<LeaderboardEntry>(task.Result.Results);
                    }
                    loadComplete = true;
                });
            
            // 로드 완료 대기 (최대 5초)
            float timeout = 5f;
            while (!loadComplete && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }
            
            if (loadError != null)
            {
                Debug.LogWarning($"순위 {offset} 로드 실패: {loadError.Message}");
            }
            else if (entries != null)
            {
                resultList.AddRange(entries);
            }
        }

        /// <summary>
        /// 하이스코어 확인 및 업데이트
        /// </summary>
        private void CheckAndUpdateHighScore(int score, float playTime)
        {
            int previousHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            
            if (score > previousHighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
                PlayerPrefs.SetFloat(BEST_TIME_KEY, playTime);
                PlayerPrefs.Save();
                
                isNewRecord = true;
                Debug.Log($"새로운 하이스코어! {previousHighScore} → {score}");
            }
            else
            {
                isNewRecord = false;
            }

            if (newRecordIndicator != null)
            {
                newRecordIndicator.SetActive(isNewRecord);
            }
        }

        /// <summary>
        /// 현재 게임 정보 UI 업데이트
        /// </summary>
        private void UpdateCurrentGameUI()
        {
            if (currentScoreText != null)
            {
                currentScoreText.text = $"SCORE : {currentGameScore}";
            }

            if (currentTimeText != null)
            {
                currentTimeText.text = $"TIME : {FormatTime(currentGameTime)}";
            }

            if (highScoreText != null)
            {
                int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
                highScoreText.text = highScore > 0 ? $"HIGH SCORE : {highScore}" : "HIGH SCORE : -";
            }

            Debug.Log($"현재 게임 UI 업데이트 완료 - 점수: {currentGameScore}, 시간: {FormatTime(currentGameTime)}");
        }

        private string FormatTime(float timeInSeconds)
        {
            if (timeInSeconds <= 0) return "00:00";
            
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        private void IncrementGameCount()
        {
            int currentCount = PlayerPrefs.GetInt(TOTAL_GAMES_KEY, 0);
            PlayerPrefs.SetInt(TOTAL_GAMES_KEY, currentCount + 1);
            PlayerPrefs.Save();
            
            Debug.Log($"총 게임 플레이 횟수: {currentCount + 1}회");
        }

        private void DisplayRankings(List<LeaderboardEntry> entries, int myRank)
        {
            ClearRankingTexts();
            
            foreach (var entry in entries)
            {
                if (entry.Rank == myRank - 1 && upperRankText != null)
                {
                    // 상위 순위 표시
                    upperRankText.text = (entry.Rank + 1).ToString();
                    upperNameText.text = entry.PlayerName;
                    upperScoreText.text = entry.Score.ToString();
                }
                else if (entry.Rank == myRank && myRankText != null)
                {
                    // 내 순위 표시
                    myRankText.text = (entry.Rank + 1).ToString();
                    myNameText.text = entry.PlayerName;
                    myScoreText.text = entry.Score.ToString();
                }
                else if (entry.Rank == myRank + 1 && lowerRankText != null)
                {
                    // 하위 순위 표시
                    lowerRankText.text = (entry.Rank + 1).ToString();
                    lowerNameText.text = entry.PlayerName;
                    lowerScoreText.text = entry.Score.ToString();
                }
            }
        }

        private void ClearRankingTexts()
        {
            if (upperRankText != null) upperRankText.text = "-";
            if (upperNameText != null) upperNameText.text = "-";
            if (upperScoreText != null) upperScoreText.text = "-";
            
            if (myRankText != null) myRankText.text = "-";
            if (myNameText != null) myNameText.text = "-";
            if (myScoreText != null) myScoreText.text = "-";
            
            if (lowerRankText != null) lowerRankText.text = "-";
            if (lowerNameText != null) lowerNameText.text = "-";
            if (lowerScoreText != null) lowerScoreText.text = "-";
        }

        private void GoToMainMenu()
        {
            Close();
            PanelManager.CloseAll();
            PanelManager.Open("main");
        }

        private void RetryGame()
        {
            Close();
            Debug.Log("게임 재시작");
            // 필요하다면 여기에 게임 재시작 로직 추가
            // 예: SceneManager.LoadScene("GameScene");
        }

        private void ShowError(string message)
        {
            ErrorMenu errorMenu = (ErrorMenu)PanelManager.GetSingleton("error");
            if (errorMenu != null)
            {
                errorMenu.Open(ErrorMenu.Action.None, message, "확인");
            }
        }

        public GameStats GetGameStats()
        {
            return new GameStats
            {
                CurrentScore = currentGameScore,
                CurrentTime = currentGameTime,
                HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0),
                BestTime = PlayerPrefs.GetFloat(BEST_TIME_KEY, 0f),
                TotalGames = PlayerPrefs.GetInt(TOTAL_GAMES_KEY, 0),
                IsNewRecord = isNewRecord
            };
        }

        [ContextMenu("하이스코어 리셋")]
        public void ResetHighScore()
        {
            PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
            PlayerPrefs.DeleteKey(BEST_TIME_KEY);
            PlayerPrefs.DeleteKey(TOTAL_GAMES_KEY);
            PlayerPrefs.Save();
            
            if (highScoreText != null)
                highScoreText.text = "---";
            
            Debug.Log("하이스코어가 리셋되었습니다.");
        }

        #if UNITY_EDITOR
        [ContextMenu("테스트 - 온라인 랜덤 점수")]
        public void TestShowRandomScore()
        {
            int randomScore = UnityEngine.Random.Range(100, 1000);
            float randomTime = UnityEngine.Random.Range(30f, 300f);
            ShowRankingAfterScore(randomScore, randomTime);
        }
        
        [ContextMenu("테스트 - 오프라인 모드")]
        public void TestOfflineMode()
        {
            int randomScore = UnityEngine.Random.Range(100, 1000);
            float randomTime = UnityEngine.Random.Range(30f, 300f);
            ShowOfflineGameOver(randomScore, randomTime, "테스트 오프라인 모드입니다.");
        }
        #endif
    }

    [System.Serializable]
    public struct GameStats
    {
        public int CurrentScore;
        public float CurrentTime;
        public int HighScore;
        public float BestTime;
        public int TotalGames;
        public bool IsNewRecord;
    }
}