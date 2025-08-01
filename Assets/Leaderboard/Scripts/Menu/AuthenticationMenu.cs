using Leaderboard.Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboard.Scripts.Menu
{
    public class AuthenticationMenu : Panel
    {
        [Header("플레이어 이름 입력")]
        [SerializeField] private TMP_InputField playerNameInput = null;
        [SerializeField] private Button confirmButton = null;

        // PlayerPrefs 키 상수
        private const string PLAYER_NAME_KEY = "SavedPlayerName";
        private const string HAS_REGISTERED_KEY = "HasRegistered";

        public override void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            // 플레이어 이름 확인 버튼 이벤트 등록
            confirmButton.onClick.AddListener(OnConfirmPlayerName);
            
            base.Initialize();
        }

        public override void Open()
        {
            // 이미 등록된 플레이어인지 확인
            if (PlayerPrefs.GetInt(HAS_REGISTERED_KEY, 0) == 1)
            {
                // 이미 등록된 경우 자동으로 로그인 진행
                string savedName = PlayerPrefs.GetString(PLAYER_NAME_KEY, "Player");
                Debug.Log($"기존 플레이어 감지됨: {savedName}");
                AutoLogin(savedName);
                return;
            }
            
            // 첫 실행인 경우 이름 입력 UI 표시
            playerNameInput.text = "";
            base.Open();
        }

        /// <summary>
        /// 플레이어 이름 확인 버튼 클릭 시 호출
        /// </summary>
        private void OnConfirmPlayerName()
        {
            string playerName = playerNameInput.text.Trim();
            
            // 이름 유효성 검사
            if (string.IsNullOrEmpty(playerName))
            {
                ShowError("플레이어 이름을 입력해주세요.");
                return;
            }
            
            if (playerName.Length < 2 || playerName.Length > 12)
            {
                ShowError("이름은 2자 이상 12자 이하로 입력해주세요.");
                return;
            }
            
            // 이름 저장 및 로그인 진행
            SavePlayerName(playerName);
            MenuManager.Singleton.SignInAnonymouslyAsyncWithName(playerName);
        }

        /// <summary>
        /// 플레이어 이름을 로컬에 저장
        /// </summary>
        private void SavePlayerName(string name)
        {
            PlayerPrefs.SetString(PLAYER_NAME_KEY, name);
            PlayerPrefs.SetInt(HAS_REGISTERED_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log($"플레이어 이름 저장됨: {name}");
        }

        /// <summary>
        /// 기존 플레이어 자동 로그인
        /// </summary>
        private void AutoLogin(string playerName)
        {
            // 로딩 화면 표시 후 자동 로그인
            PanelManager.Close("auth");
            MenuManager.Singleton.SignInAnonymouslyAsyncWithName(playerName);
        }

        /// <summary>
        /// 에러 메시지 표시
        /// </summary>
        private void ShowError(string message)
        {
            ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
            panel.Open(ErrorMenu.Action.None, message, "확인");
        }
    }
}