using System;
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
    public class LeaderboardsMenu : Panel
    {
        [Header("리더보드 설정")]
        [SerializeField] private int playersPerPage = 25;
        [SerializeField] private string leaderboardId = "LeaderboardFish";
        
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
            
            // 서비스 상태 확인 후 로드
            CheckServicesAndLoadPlayers();
        }

        /// <summary>
        /// Unity Services 상태 확인 후 플레이어 로드
        /// </summary>
        private async void CheckServicesAndLoadPlayers()
        {
            try
            {
                // Unity Services 초기화 상태 확인
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    Debug.LogError("Unity Services가 초기화되지 않았습니다.");
                    ShowError("서비스가 초기화되지 않았습니다. 메인 메뉴에서 다시 시도해주세요.");
                    return;
                }

                // 인증 상태 확인
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.LogError("사용자가 로그인되지 않았습니다.");
                    ShowError("로그인이 필요합니다. 메인 메뉴에서 다시 시도해주세요.");
                    return;
                }

                Debug.Log($"서비스 상태 확인 완료. 리더보드 ID: {leaderboardId}");
                Debug.Log($"인증된 플레이어: {AuthenticationService.Instance.PlayerName}");
                
                LoadPlayers(1);
            }
            catch (Exception exception)
            {
                Debug.LogError($"서비스 상태 확인 실패: {exception.Message}");
                ShowError("서비스 연결에 문제가 있습니다. 잠시 후 다시 시도해주세요.");
            }
        }

        private async void LoadPlayers(int page)
        {
            nextButton.interactable = false;
            prevButton.interactable = false;
            
            try
            {
                Debug.Log($"리더보드 로드 시작 - 페이지: {page}, ID: {leaderboardId}");
                
                GetScoresOptions options = new GetScoresOptions
                {
                    Offset = (page - 1) * playersPerPage,
                    Limit = playersPerPage
                };
                
                var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);
                
                ClearPlayersList();
                
                Debug.Log($"리더보드 데이터 수신: {scores.Results.Count}명의 플레이어");
                
                for (int i = 0; i < scores.Results.Count; i++)
                {
                    LeaderboardsPlayerItem item = Instantiate(playerItemPrefab, playersContainer);
                    item.Initialize(scores.Results[i]);
                }
                
                totalPages = Mathf.CeilToInt((float)scores.Total / (float)scores.Limit);
                currentPage = page;
                
                Debug.Log($"리더보드 로드 완료. 총 {scores.Results.Count}명, 페이지 {currentPage}/{totalPages}");
            }
            catch (Unity.Services.Leaderboards.Exceptions.LeaderboardsException leaderboardException)
            {
                Debug.LogError($"리더보드 전용 에러: {leaderboardException.Message}");
                Debug.LogError($"에러 코드: {leaderboardException.ErrorCode}");
                
                if (leaderboardException.ErrorCode == (int)Unity.Services.Leaderboards.Exceptions.LeaderboardsExceptionReason.LeaderboardNotFound)
                {
                    ShowError($"리더보드를 찾을 수 없습니다. (ID: {leaderboardId})");
                }
                else if (leaderboardException.ErrorCode == (int)Unity.Services.Leaderboards.Exceptions.LeaderboardsExceptionReason.Unauthorized)
                {
                    ShowError("리더보드 접근 권한이 없습니다. 다시 로그인해주세요.");
                }
                else
                {
                    ShowError($"리더보드 오류: {leaderboardException.Message}");
                }
            }
            catch (Unity.Services.Core.RequestFailedException requestException)
            {
                Debug.LogError($"네트워크 요청 실패: {requestException.Message}");
                Debug.LogError($"HTTP 상태 코드: {requestException.ErrorCode}");
                ShowError("네트워크 연결을 확인해주세요.");
            }
            catch (Exception exception)
            {
                Debug.LogError($"리더보드 로드 실패: {exception.Message}");
                Debug.LogError($"에러 타입: {exception.GetType().Name}");
                Debug.LogError($"스택 트레이스: {exception.StackTrace}");
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