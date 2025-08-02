using System;
using System.Collections.Generic;
using Leaderboard.Scripts.Tools;
using TMPro;
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
        [SerializeField] private GameObject newRecordIndicator; // "NEW RECORD!" 표시용
        
        [Header("버튼")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button retryButton;

        // PlayerPrefs 키 상수
        private const string HIGH_SCORE_KEY = "HighScore";
        private const string BEST_TIME_KEY = "BestTime";
        private const string TOTAL_GAMES_KEY = "TotalGames";

        // 현재 게임 정보 저장
        private int currentGameScore = 0;
        private float currentGameTime = 0f;
        private bool isNewRecord = false;

        public override void Initialize()
        {
            if (IsInitialized) return;
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            
            if (retryButton != null)
                retryButton.onClick.AddListener(RetryGame);
            
            // 새 기록 표시기 초기 비활성화
            if (newRecordIndicator != null)
                newRecordIndicator.SetActive(false);
            
            base.Initialize();
        }

        /// <summary>
        /// 게임 종료 후 점수와 시간으로 랭킹 표시
        /// </summary>
        public async void ShowRankingAfterScore(int score, float playTime)
        {
            try
            {
                Debug.Log($"점수 업로드 및 랭킹 표시 시작: {score}점, {playTime:F2}초");
                
                // 현재 게임 정보 저장
                currentGameScore = score;
                currentGameTime = playTime;
                
                // 하이스코어 확인 및 업데이트
                CheckAndUpdateHighScore(score, playTime);
                
                // 현재 게임 정보 UI 업데이트
                UpdateCurrentGameUI();
                
                // 리더보드에 점수 업로드
                var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(
                    leaderboardId, 
                    score
                );
                
                Debug.Log($"점수 업로드 성공! 내 순위: {playerEntry.Rank + 1}");
                
                // 주변 순위 로드
                await LoadSurroundingRanks(playerEntry.Rank);
                
                // 게임 횟수 증가
                IncrementGameCount();
                
                Open();
            }
            catch (Exception exception)
            {
                Debug.LogError($"랭킹 표시 실패: {exception.Message}");
                ShowError("랭킹 정보를 불러올 수 없습니다.");
                
                // 에러가 발생해도 현재 게임 정보는 표시
                currentGameScore = score;
                currentGameTime = playTime;
                CheckAndUpdateHighScore(score, playTime);
                UpdateCurrentGameUI();
                Open();
            }
        }

        /// <summary>
        /// 하이스코어 확인 및 업데이트
        /// </summary>
        private void CheckAndUpdateHighScore(int score, float playTime)
        {
            int previousHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            
            // 새로운 하이스코어인지 확인
            if (score > previousHighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
                PlayerPrefs.SetFloat(BEST_TIME_KEY, playTime); // 하이스코어와 함께 그때의 시간도 저장
                PlayerPrefs.Save();
                
                isNewRecord = true;
                Debug.Log($"새로운 하이스코어! {previousHighScore} → {score}");
            }
            else
            {
                isNewRecord = false;
            }

            // 새 기록 표시기 활성화/비활성화
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
            // 현재 점수 표시
            if (currentScoreText != null)
            {
                currentScoreText.text = currentGameScore.ToString();
            }

            // 현재 시간 표시 (분:초 형식)
            if (currentTimeText != null)
            {
                currentTimeText.text = FormatTime(currentGameTime);
            }

            // 하이스코어 표시
            if (highScoreText != null)
            {
                int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
                if (highScore > 0)
                {
                    highScoreText.text = highScore.ToString();
                }
                else
                {
                    highScoreText.text = "---";
                }
            }

            Debug.Log($"현재 게임 UI 업데이트 완료 - 점수: {currentGameScore}, 시간: {FormatTime(currentGameTime)}, 하이스코어: {PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0)}");
        }

        /// <summary>
        /// 시간을 "분:초" 형식으로 변환
        /// </summary>
        private string FormatTime(float timeInSeconds)
        {
            if (timeInSeconds <= 0) return "00:00";
            
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// 게임 플레이 횟수 증가
        /// </summary>
        private void IncrementGameCount()
        {
            int currentCount = PlayerPrefs.GetInt(TOTAL_GAMES_KEY, 0);
            PlayerPrefs.SetInt(TOTAL_GAMES_KEY, currentCount + 1);
            PlayerPrefs.Save();
            
            Debug.Log($"총 게임 플레이 횟수: {currentCount + 1}회");
        }

        private async System.Threading.Tasks.Task LoadSurroundingRanks(int myRank)
        {
            try
            {
                List<LeaderboardEntry> allEntries = new List<LeaderboardEntry>();
                
                if (myRank > 0)
                {
                    var upperOptions = new GetScoresOptions
                    {
                        Offset = myRank - 1,
                        Limit = 1
                    };
                    var upperScores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, upperOptions);
                    if (upperScores.Results.Count > 0)
                    {
                        allEntries.Add(upperScores.Results[0]);
                    }
                }
                
                var myOptions = new GetScoresOptions
                {
                    Offset = myRank,
                    Limit = 1
                };
                var myScores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, myOptions);
                if (myScores.Results.Count > 0)
                {
                    allEntries.Add(myScores.Results[0]);
                }
                
                var lowerOptions = new GetScoresOptions
                {
                    Offset = myRank + 1,
                    Limit = 1
                };
                var lowerScores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, lowerOptions);
                if (lowerScores.Results.Count > 0)
                {
                    allEntries.Add(lowerScores.Results[0]);
                }
                
                DisplayRankings(allEntries, myRank);
            }
            catch (Exception exception)
            {
                Debug.LogError($"주변 랭킹 로드 실패: {exception.Message}");
                throw;
            }
        }

        private void DisplayRankings(List<LeaderboardEntry> entries, int myRank)
        {
            // 모든 텍스트 초기화
            ClearRankingTexts();
            
            string currentPlayerName = PlayerPrefs.GetString("SavedPlayerName", "");
            
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

        /// <summary>
        /// 순위 텍스트들 초기화
        /// </summary>
        private void ClearRankingTexts()
        {
            if (upperRankText != null) upperRankText.text = "---";
            if (upperNameText != null) upperNameText.text = "---";
            if (upperScoreText != null) upperScoreText.text = "---";
            
            if (myRankText != null) myRankText.text = "---";
            if (myNameText != null) myNameText.text = "---";
            if (myScoreText != null) myScoreText.text = "---";
            
            if (lowerRankText != null) lowerRankText.text = "---";
            if (lowerNameText != null) lowerNameText.text = "---";
            if (lowerScoreText != null) lowerScoreText.text = "---";
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
            // 게임 재시작 로직 (GameManager나 SceneManager 호출)
            Debug.Log("게임 재시작");
        }

        private void ShowError(string message)
        {
            ErrorMenu errorMenu = (ErrorMenu)PanelManager.GetSingleton("error");
            if (errorMenu != null)
            {
                errorMenu.Open(ErrorMenu.Action.None, message, "확인");
            }
        }

        /// <summary>
        /// 통계 정보 가져오기 (다른 스크립트에서 사용 가능)
        /// </summary>
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

        /// <summary>
        /// 하이스코어 리셋 (디버그/테스트용)
        /// </summary>
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
        /// <summary>
        /// 에디터에서 테스트용
        /// </summary>
        [ContextMenu("테스트 - 랜덤 점수로 표시")]
        public void TestShowRandomScore()
        {
            int randomScore = UnityEngine.Random.Range(100, 1000);
            float randomTime = UnityEngine.Random.Range(30f, 300f);
            ShowRankingAfterScore(randomScore, randomTime);
        }
        #endif
    }

    /// <summary>
    /// 게임 통계 정보 구조체
    /// </summary>
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