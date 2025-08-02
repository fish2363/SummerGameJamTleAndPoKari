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
        [SerializeField] private string leaderboardId = "2025SummerGameJam";
        
        [Header("UI 요소")]
        [SerializeField] private GameObject upperRankContainer;
        [SerializeField] private GameObject myRankContainer;
        [SerializeField] private GameObject lowerRankContainer;
        
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
        
        [Header("버튼")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button retryButton;

        public override void Initialize()
        {
            if (IsInitialized) return;
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            
            if (retryButton != null)
                retryButton.onClick.AddListener(RetryGame);
            
            base.Initialize();
        }

        public async void ShowRankingAfterScore(int score, float playTime)
        {
            try
            {
                Debug.Log($"점수 업로드 및 랭킹 표시 시작: {score}점");
                
                var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(
                    leaderboardId, 
                    score
                );
                
                Debug.Log($"점수 업로드 성공! 내 순위: {playerEntry.Rank + 1}");
                
                await LoadSurroundingRanks(playerEntry.Rank);
                
                Open();
            }
            catch (Exception exception)
            {
                Debug.LogError($"랭킹 표시 실패: {exception.Message}");
                ShowError("랭킹 정보를 불러올 수 없습니다.");
            }
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
            upperRankContainer.SetActive(false);
            myRankContainer.SetActive(false);
            lowerRankContainer.SetActive(false);
            
            string currentPlayerName = PlayerPrefs.GetString("SavedPlayerName", "");
            
            foreach (var entry in entries)
            {
                if (entry.Rank == myRank - 1)
                {
                    upperRankContainer.SetActive(true);
                    upperRankText.text = (entry.Rank + 1).ToString();
                    upperNameText.text = entry.PlayerName;
                    upperScoreText.text = entry.Score.ToString();
                }
                else if (entry.Rank == myRank)
                {
                    myRankContainer.SetActive(true);
                    myRankText.text = (entry.Rank + 1).ToString();
                    myNameText.text = entry.PlayerName;
                    myScoreText.text = entry.Score.ToString();
                }
                else if (entry.Rank == myRank + 1)
                {
                    lowerRankContainer.SetActive(true);
                    lowerRankText.text = (entry.Rank + 1).ToString();
                    lowerNameText.text = entry.PlayerName;
                    lowerScoreText.text = entry.Score.ToString();
                }
            }
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
    }
}