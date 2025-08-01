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
    /// <summary>
    /// 리더보드 UI를 관리하는 메뉴
    /// 점수와 시간을 함께 표시
    /// </summary>
    public class LeaderboardsMenu : Panel
    {
        [Header("리더보드 설정")]
        [SerializeField] private int playersPerPage = 25;
        [SerializeField] private string leaderboardId = "2025SummerGameJam"; // 리더보드 ID
        
        [Header("UI 요소")]
        [SerializeField] private LeaderboardsPlayerItem playerItemPrefab = null;
        [SerializeField] private RectTransform playersContainer = null;
        [SerializeField] public TextMeshProUGUI pageText = null;
        [SerializeField] private Button nextButton = null;
        [SerializeField] private Button prevButton = null;
        [SerializeField] private Button closeButton = null;

        [SerializeField] private Button refreshButton = null; // 새로고침 버튼 추가

        private int currentPage = 1;
        private int totalPages = 0;

        public override void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            ClearPlayersList();
            closeButton.onClick.AddListener(ClosePanel);
            nextButton.onClick.AddListener(NextPage);
            prevButton.onClick.AddListener(PrevPage);
            
            // 새로고침 버튼
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
    
        /// <summary>
        /// 게임 오버 시 점수 업로드 (외부에서 호출)
        /// 메타데이터 없이 점수만 업로드
        /// </summary>
        public async void UploadScore(int highScore, float playTime)
        {
            try
            {
                Debug.Log($"리더보드에 점수 업로드 중... 점수: {highScore}");
                
                // 메타데이터 없이 점수만 업로드
                var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(
                    leaderboardId, 
                    highScore
                );
                
                Debug.Log($"점수 업로드 성공! 순위: {playerEntry.Rank + 1}");
                
                // 업로드 후 리더보드 새로고침
                if (IsOpen)
                {
                    LoadPlayers(currentPage);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"점수 업로드 실패: {exception.Message}");
                ShowError("점수 업로드에 실패했습니다.");
            }
        }

        /// <summary>
        /// 리더보드 데이터 로드
        /// </summary>
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
                    // IncludeMetadata 제거 - 메타데이터 사용하지 않음
                };
                
                var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);
                
                ClearPlayersList();
                
                // 각 플레이어 항목 생성
                for (int i = 0; i < scores.Results.Count; i++)
                {
                    LeaderboardsPlayerItem item = Instantiate(playerItemPrefab, playersContainer);
                    
                    // 메타데이터 없이 기본 초기화 사용
                    item.Initialize(scores.Results[i]);
                }
                
                // 페이지 정보 업데이트
                totalPages = Mathf.CeilToInt((float)scores.Total / (float)scores.Limit);
                currentPage = page;
                
                Debug.Log($"리더보드 로드 완료. 총 {scores.Results.Count}명, 페이지 {currentPage}/{totalPages}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"리더보드 로드 실패: {exception.Message}");
                ShowError("리더보드를 불러올 수 없습니다.");
            }
            
            // 페이지 텍스트 및 버튼 상태 업데이트
            pageText.text = currentPage.ToString() + "/" + totalPages.ToString();
            nextButton.interactable = currentPage < totalPages && totalPages > 1;
            prevButton.interactable = currentPage > 1 && totalPages > 1;
        }

        /// <summary>
        /// 다음 페이지로 이동
        /// </summary>
        private void NextPage()
        {
            if (currentPage + 1 > totalPages)
            {
                LoadPlayers(1); // 마지막 페이지에서 다음 누르면 첫 페이지로
            }
            else
            {
                LoadPlayers(currentPage + 1);
            }
        }

        /// <summary>
        /// 이전 페이지로 이동
        /// </summary>
        private void PrevPage()
        {
            if (currentPage - 1 <= 0)
            {
                LoadPlayers(totalPages); // 첫 페이지에서 이전 누르면 마지막 페이지로
            }
            else
            {
                LoadPlayers(currentPage - 1);
            }
        }

        /// <summary>
        /// 패널 닫기
        /// </summary>
        private void ClosePanel()
        {
            Close();
        }

        /// <summary>
        /// 플레이어 목록 초기화
        /// </summary>
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
        
        /// <summary>
        /// 에러 메시지 표시
        /// </summary>
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