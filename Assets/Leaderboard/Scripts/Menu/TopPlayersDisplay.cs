using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace Leaderboard.Scripts.Menu
{
    public class TopPlayersDisplay : MonoBehaviour
    {
        [Header("리더보드 설정")]
        [SerializeField] private string leaderboardId = "LeaderboardFish";
        
        [Header("플레이어 이름 UI")]
        [SerializeField] private TextMeshProUGUI myPlayerNameText;
        
        [Header("상위 3명 UI")]
        [SerializeField] private TextMeshProUGUI firstPlaceText;
        [SerializeField] private TextMeshProUGUI secondPlaceText;
        [SerializeField] private TextMeshProUGUI thirdPlaceText;
        
        [Header("상위 3명 점수 UI (선택사항)")]
        [SerializeField] private TextMeshProUGUI firstPlaceScoreText;
        [SerializeField] private TextMeshProUGUI secondPlaceScoreText;
        [SerializeField] private TextMeshProUGUI thirdPlaceScoreText;
        
        [Header("설정")]
        [SerializeField] private bool showScores = false;
        [SerializeField] private bool autoRefreshOnStart = true;
        [SerializeField] private float autoRefreshInterval = 30f; // 30초마다 자동 갱신
        
        [Header("기본 텍스트")]
        [SerializeField] private string defaultPlayerName = "-";
        [SerializeField] private string noPlayerText = "-";
        
        private bool isInitialized = false;
        
        void Start()
        {
            Initialize();
            
            if (autoRefreshOnStart)
            {
                RefreshPlayerNames();
                
                // 자동 갱신 시작
                if (autoRefreshInterval > 0)
                {
                    InvokeRepeating(nameof(RefreshPlayerNames), autoRefreshInterval, autoRefreshInterval);
                }
            }
        }
        
        void OnDestroy()
        {
            // 자동 갱신 중지
            CancelInvoke();
        }
        
        /// <summary>
        /// 컴포넌트 초기화
        /// </summary>
        public void Initialize()
        {
            if (isInitialized) return;
            
            // 기본 텍스트 설정
            SetDefaultTexts();
            
            isInitialized = true;
            Debug.Log("TopPlayersDisplay 초기화 완료");
        }
        
        /// <summary>
        /// 플레이어 이름들을 새로고침
        /// </summary>
        public async void RefreshPlayerNames()
        {
            try
            {
                // Unity Services 상태 확인
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    Debug.LogWarning("Unity Services가 초기화되지 않았습니다.");
                    SetDefaultTexts();
                    return;
                }
                
                // 내 플레이어 이름 설정
                UpdateMyPlayerName();
                
                // 상위 3명 가져오기
                await UpdateTop3Players();
                
                Debug.Log("플레이어 이름 새로고침 완료");
            }
            catch (Exception exception)
            {
                Debug.LogError($"플레이어 이름 새로고침 실패: {exception.Message}");
                SetDefaultTexts();
            }
        }
        
        /// <summary>
        /// 내 플레이어 이름 업데이트
        /// </summary>
        private void UpdateMyPlayerName()
        {
            if (myPlayerNameText == null) return;
            
            string playerName = "";
            
            // 1. Unity Authentication에서 플레이어 이름 가져오기
            if (AuthenticationService.Instance.IsSignedIn && 
                !string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                playerName = AuthenticationService.Instance.PlayerName;
            }
            // 2. PlayerPrefs에서 저장된 이름 가져오기
            else
            {
                playerName = PlayerPrefs.GetString("SavedPlayerName", defaultPlayerName);
            }
            
            myPlayerNameText.text = playerName;
            Debug.Log($"내 플레이어 이름 업데이트: {playerName}");
        }
        
        /// <summary>
        /// 상위 3명 플레이어 정보 업데이트
        /// </summary>
        private async System.Threading.Tasks.Task UpdateTop3Players()
        {
            try
            {
                // 상위 3명 가져오기
                GetScoresOptions options = new GetScoresOptions
                {
                    Offset = 0,
                    Limit = 3
                };
                
                var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);
                
                // UI 업데이트
                UpdateTop3UI(scores.Results);
                
                Debug.Log($"상위 3명 정보 업데이트 완료: {scores.Results.Count}명");
            }
            catch (Unity.Services.Leaderboards.Exceptions.LeaderboardsException leaderboardException)
            {
                Debug.LogError($"리더보드 에러: {leaderboardException.Message} (코드: {leaderboardException.ErrorCode})");
                SetTop3DefaultTexts();
            }
            catch (Exception exception)
            {
                Debug.LogError($"상위 3명 정보 가져오기 실패: {exception.Message}");
                SetTop3DefaultTexts();
            }
        }
        
        /// <summary>
        /// 상위 3명 UI 업데이트
        /// </summary>
        private void UpdateTop3UI(List<LeaderboardEntry> topPlayers)
        {
            // 1등
            if (topPlayers.Count > 0 && firstPlaceText != null)
            {
                firstPlaceText.text = topPlayers[0].PlayerName;
                if (showScores && firstPlaceScoreText != null)
                {
                    firstPlaceScoreText.text = topPlayers[0].Score.ToString();
                }
            }
            else
            {
                if (firstPlaceText != null) firstPlaceText.text = noPlayerText;
                if (firstPlaceScoreText != null) firstPlaceScoreText.text = "0";
            }
            
            // 2등
            if (topPlayers.Count > 1 && secondPlaceText != null)
            {
                secondPlaceText.text = topPlayers[1].PlayerName;
                if (showScores && secondPlaceScoreText != null)
                {
                    secondPlaceScoreText.text = topPlayers[1].Score.ToString();
                }
            }
            else
            {
                if (secondPlaceText != null) secondPlaceText.text = noPlayerText;
                if (secondPlaceScoreText != null) secondPlaceScoreText.text = "0";
            }
            
            // 3등
            if (topPlayers.Count > 2 && thirdPlaceText != null)
            {
                thirdPlaceText.text = topPlayers[2].PlayerName;
                if (showScores && thirdPlaceScoreText != null)
                {
                    thirdPlaceScoreText.text = topPlayers[2].Score.ToString();
                }
            }
            else
            {
                if (thirdPlaceText != null) thirdPlaceText.text = noPlayerText;
                if (thirdPlaceScoreText != null) thirdPlaceScoreText.text = "0";
            }
        }
        
        /// <summary>
        /// 기본 텍스트 설정
        /// </summary>
        private void SetDefaultTexts()
        {
            // 내 플레이어 이름 기본값
            if (myPlayerNameText != null)
            {
                string savedName = PlayerPrefs.GetString("SavedPlayerName", defaultPlayerName);
                myPlayerNameText.text = savedName;
            }
            
            // 상위 3명 기본값
            SetTop3DefaultTexts();
        }
        
        /// <summary>
        /// 상위 3명 기본 텍스트 설정
        /// </summary>
        private void SetTop3DefaultTexts()
        {
            if (firstPlaceText != null) firstPlaceText.text = noPlayerText;
            if (secondPlaceText != null) secondPlaceText.text = noPlayerText;
            if (thirdPlaceText != null) thirdPlaceText.text = noPlayerText;
            
            if (showScores)
            {
                if (firstPlaceScoreText != null) firstPlaceScoreText.text = "0";
                if (secondPlaceScoreText != null) secondPlaceScoreText.text = "0";
                if (thirdPlaceScoreText != null) thirdPlaceScoreText.text = "0";
            }
        }
        
        /// <summary>
        /// 수동으로 새로고침 (버튼 등에서 호출 가능)
        /// </summary>
        public void ManualRefresh()
        {
            RefreshPlayerNames();
        }
        
        /// <summary>
        /// 자동 새로고침 시작
        /// </summary>
        public void StartAutoRefresh()
        {
            if (autoRefreshInterval > 0)
            {
                CancelInvoke();
                InvokeRepeating(nameof(RefreshPlayerNames), 0f, autoRefreshInterval);
                Debug.Log($"자동 새로고침 시작: {autoRefreshInterval}초 간격");
            }
        }
        
        /// <summary>
        /// 자동 새로고침 중지
        /// </summary>
        public void StopAutoRefresh()
        {
            CancelInvoke();
            Debug.Log("자동 새로고침 중지");
        }
        
        /// <summary>
        /// 리더보드 ID 변경
        /// </summary>
        public void SetLeaderboardId(string newLeaderboardId)
        {
            if (!string.IsNullOrEmpty(newLeaderboardId))
            {
                leaderboardId = newLeaderboardId;
                Debug.Log($"리더보드 ID 변경: {leaderboardId}");
                RefreshPlayerNames();
            }
        }
        
        /// <summary>
        /// 점수 표시 여부 설정
        /// </summary>
        public void SetShowScores(bool show)
        {
            showScores = show;
            
            // 점수 텍스트 활성화/비활성화
            if (firstPlaceScoreText != null) firstPlaceScoreText.gameObject.SetActive(show);
            if (secondPlaceScoreText != null) secondPlaceScoreText.gameObject.SetActive(show);
            if (thirdPlaceScoreText != null) thirdPlaceScoreText.gameObject.SetActive(show);
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// 에디터에서 테스트용 (Inspector에서 버튼으로 호출 가능)
        /// </summary>
        [ContextMenu("테스트 새로고침")]
        public void TestRefresh()
        {
            RefreshPlayerNames();
        }
        
        [ContextMenu("기본값 설정")]
        public void TestSetDefaults()
        {
            SetDefaultTexts();
        }
        #endif
    }
}