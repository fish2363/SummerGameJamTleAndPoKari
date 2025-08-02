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
    public class LeaderboardsMenu : Panel
    {
        [Header("리더보드 설정")]
        [SerializeField] private int playersPerPage = 25;
        [SerializeField] private string leaderboardId = "2025SummerGameJam";
        
        [Header("UI 요소")]
        [SerializeField] private LeaderboardsPlayerItem playerItemPrefab = null;
        [SerializeField] private RectTransform playersContainer = null;
        [SerializeField] public TextMeshProUGUI pageText = null;
        [SerializeField] private Button nextButton = null;
        [SerializeField] private Button prevButton = null;
        [SerializeField] private Button closeButton = null;
        [SerializeField] private Button refreshButton = null;

        private int currentPage = 1;
        private int totalPages = 0;

        public override void Initialize()
        {
            if (IsInitialized) return;
            
            ClearPlayersList();
            closeButton.onClick.AddListener(ClosePanel);
            nextButton.onClick.AddListener(NextPage);
            prevButton.onClick.AddListener(PrevPage);
            
            if (refreshButton != null)
            {
                refreshButton.onClick.AddListener(() => LoadPlayers(currentPage));
            }
            
            base.Initialize();
        }
    
        public override void Open()
        {
            pageText.text = "-";
            nextButton.interactable = false;
            prevButton.interactable = false;
            base.Open();
            ClearPlayersList();
            currentPage = 1;
            totalPages = 0;
            LoadPlayers(1);
        }

        private async void LoadPlayers(int page)
        {
            nextButton.interactable = false;
            prevButton.interactable = false;
            
            try
            {
                GetScoresOptions options = new GetScoresOptions
                {
                    Offset = (page - 1) * playersPerPage,
                    Limit = playersPerPage
                };
                
                var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);
                
                ClearPlayersList();
                
                for (int i = 0; i < scores.Results.Count; i++)
                {
                    LeaderboardsPlayerItem item = Instantiate(playerItemPrefab, playersContainer);
                    item.Initialize(scores.Results[i]);
                }
                
                totalPages = Mathf.CeilToInt((float)scores.Total / (float)scores.Limit);
                currentPage = page;
                
                Debug.Log($"리더보드 로드 완료. 총 {scores.Results.Count}명, 페이지 {currentPage}/{totalPages}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"리더보드 로드 실패: {exception.Message}");
                ShowError("리더보드를 불러올 수 없습니다.");
            }
            
            pageText.text = currentPage.ToString() + "/" + totalPages.ToString();
            nextButton.interactable = currentPage < totalPages && totalPages > 1;
            prevButton.interactable = currentPage > 1 && totalPages > 1;
        }

        private void NextPage()
        {
            if (currentPage + 1 > totalPages)
            {
                LoadPlayers(1);
            }
            else
            {
                LoadPlayers(currentPage + 1);
            }
        }

        private void PrevPage()
        {
            if (currentPage - 1 <= 0)
            {
                LoadPlayers(totalPages);
            }
            else
            {
                LoadPlayers(currentPage - 1);
            }
        }

        private void ClosePanel()
        {
            Close();
        }

        private void ClearPlayersList()
        {
            LeaderboardsPlayerItem[] items = playersContainer.GetComponentsInChildren<LeaderboardsPlayerItem>();
            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    Destroy(items[i].gameObject);
                }
            }
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