using System;
using Leaderboard.Scripts.Tools;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Leaderboard.Scripts.Menu
{
    /// <summary>
    /// 메뉴 시스템 전체를 관리하는 매니저
    /// 플레이어 인증 및 세션 관리 담당
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        private bool initialized = false;
        private bool eventsInitialized = false;
        
        // 현재 플레이어 이름 저장
        private string currentPlayerName = "";
        
        private static MenuManager singleton = null;

        public static MenuManager Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = FindFirstObjectByType<MenuManager>();
                    singleton.Initialize();
                }
                return singleton; 
            }
        }

        private void Initialize()
        {
            if (initialized) { return; }
            initialized = true;
        }
    
        private void OnDestroy()
        {
            if (singleton == this)
            {
                singleton = null;
            }
        }

        public void ClicktoStartClientService()
        {
            Application.runInBackground = true;
            StartClientService();
        }

        public async void StartClientService()
        {
            PanelManager.CloseAll();
            PanelManager.Open("loading");
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    var options = new InitializationOptions();
                    options.SetProfile("default_profile");
                    await UnityServices.InitializeAsync();
                }
            
                if (!eventsInitialized)
                {
                    SetupEvents();
                }

                // 기존 플레이어 확인
                if (PlayerPrefs.GetInt("HasRegistered", 0) == 1)
                {
                    // 저장된 이름으로 자동 로그인
                    string savedName = PlayerPrefs.GetString("SavedPlayerName", "Player");
                    SignInAnonymouslyAsyncWithName(savedName);
                }
                else
                {
                    // 첫 실행 - 인증 메뉴 표시
                    PanelManager.Open("auth");
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"서비스 초기화 실패: {exception.Message}");
                ShowError(ErrorMenu.Action.StartService, "네트워크 연결에 실패했습니다.", "재시도");
            }
        }

        /// <summary>
        /// 익명 로그인 (기존 메서드 - 더 이상 직접 사용하지 않음)
        /// </summary>
        public async void SignInAnonymouslyAsync()
        {
            // 기본 이름으로 로그인
            SignInAnonymouslyAsyncWithName("Player");
        }
        
        /// <summary>
        /// 플레이어 이름과 함께 익명 로그인
        /// </summary>
        public async void SignInAnonymouslyAsyncWithName(string playerName)
        {
            PanelManager.Open("loading");
            currentPlayerName = playerName;
            
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"자동 로그인 성공: {playerName}");
            }
            catch (AuthenticationException exception)
            {
                Debug.LogError($"로그인 실패: {exception.Message}");
                ShowError(ErrorMenu.Action.OpenAuthMenu, "로그인에 실패했습니다.", "확인");
            }
            catch (RequestFailedException exception)
            {
                Debug.LogError($"네트워크 오류: {exception.Message}");
                ShowError(ErrorMenu.Action.SignIn, "네트워크 연결에 실패했습니다.", "재시도");
            }
        }
    
        /// <summary>
        /// 사용자명/비밀번호 로그인 (더 이상 사용하지 않음)
        /// </summary>
        public async void SignInWithUsernameAndPasswordAsync(string username, string password)
        {
            Debug.LogWarning("사용자명/비밀번호 로그인은 더 이상 지원하지 않습니다.");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "이 기능은 더 이상 지원하지 않습니다.", "확인");
        }
    
        /// <summary>
        /// 회원가입 (더 이상 사용하지 않음)
        /// </summary>
        public async void SignUpWithUsernameAndPasswordAsync(string username, string password)
        {
            Debug.LogWarning("회원가입 기능은 더 이상 지원하지 않습니다.");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "이 기능은 더 이상 지원하지 않습니다.", "확인");
        }
    
        /// <summary>
        /// 로그아웃
        /// </summary>
        public void SignOut()
        {
            AuthenticationService.Instance.SignOut();
            // 로그아웃 시에도 저장된 이름은 유지 (다시 로그인 시 사용)
            PanelManager.CloseAll();
            PanelManager.Open("auth");
        }
    
        /// <summary>
        /// 인증 관련 이벤트 설정
        /// </summary>
        private void SetupEvents()
        {
            eventsInitialized = true;
            
            // 로그인 성공 이벤트
            AuthenticationService.Instance.SignedIn += () =>
            {
                SignInConfirmAsync();
            };

            // 로그아웃 이벤트
            AuthenticationService.Instance.SignedOut += () =>
            {
                PanelManager.CloseAll();
                PanelManager.Open("auth");
            };
        
            // 세션 만료 이벤트
            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("세션 만료 - 재로그인 시도");
                SignInAnonymouslyAsyncWithName(currentPlayerName);
            };
        }
    
        /// <summary>
        /// 에러 메시지 표시
        /// </summary>
        private void ShowError(ErrorMenu.Action action = ErrorMenu.Action.None, string error = "", string button = "")
        {
            PanelManager.Close("loading");
            ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
            panel.Open(action, error, button);
        }
    
        /// <summary>
        /// 로그인 성공 후 처리
        /// </summary>
        private async void SignInConfirmAsync()
        {
            try
            {
                // Unity 서비스의 플레이어 이름을 저장된 이름으로 업데이트
                await AuthenticationService.Instance.UpdatePlayerNameAsync(currentPlayerName);
                Debug.Log($"플레이어 이름 업데이트 완료: {currentPlayerName}");
                
                PanelManager.CloseAll();
                PanelManager.Open("main");
            }
            catch (Exception e)
            {
                Debug.LogError($"플레이어 이름 업데이트 실패: {e.Message}");
                // 실패해도 메인 메뉴로 이동
                PanelManager.CloseAll();
                PanelManager.Open("main");
            }
        }
        
        /// <summary>
        /// 현재 플레이어 이름 가져오기
        /// </summary>
        public string GetCurrentPlayerName()
        {
            return currentPlayerName;
        }
    }
}