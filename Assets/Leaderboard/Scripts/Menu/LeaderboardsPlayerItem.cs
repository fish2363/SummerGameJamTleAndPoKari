using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboard.Scripts.Menu
{
    public class LeaderboardsPlayerItem : MonoBehaviour
    {
        [Header("UI 요소")]
        [SerializeField] public TextMeshProUGUI rankText = null;
        [SerializeField] public TextMeshProUGUI nameText = null;
        [SerializeField] public TextMeshProUGUI scoreText = null;
        [SerializeField] private Button selectButton = null;
        
        [Header("하이라이트 설정")]
        [SerializeField] private Image backgroundImage = null;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color myScoreColor = new Color(1f, 0.9f, 0.3f, 0.3f); // 내 점수 하이라이트 색상
    
        private LeaderboardEntry player = null;
    
        private void Start()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(Clicked);
            }
        }

        /// <summary>
        /// 플레이어 정보 초기화 (기존 메서드 - 하위 호환성 유지)
        /// </summary>
        public void Initialize(LeaderboardEntry player)
        {
            this.player = player;
            rankText.text = (player.Rank + 1).ToString();
            nameText.text = player.PlayerName;
            scoreText.text = player.Score.ToString();
            
            // 내 점수인지 확인하여 하이라이트
            CheckIfMyScore();
        }
        
        /// <summary>
        /// 플레이어 정보와 시간을 함께 초기화
        /// </summary>
        public void InitializeWithTime(LeaderboardEntry player, string playTime)
        {
            this.player = player;
            
            // 순위 표시 (1부터 시작)
            rankText.text = (player.Rank + 1).ToString();
            
            // 플레이어 이름
            nameText.text = player.PlayerName;
            
            // 점수와 시간 표시 형식: "점수 (시간초)"
            float timeValue = 0f;
            if (float.TryParse(playTime, out timeValue))
            {
                // 시간을 분:초 형식으로 변환
                int minutes = Mathf.FloorToInt(timeValue / 60f);
                int seconds = Mathf.FloorToInt(timeValue % 60f);
                scoreText.text = $"{player.Score} ({minutes:00}:{seconds:00})";
            }
            else
            {
                // 시간 정보가 없거나 파싱 실패 시 점수만 표시
                scoreText.text = player.Score.ToString();
            }
            
            // 내 점수인지 확인하여 하이라이트
            CheckIfMyScore();
        }
        
        /// <summary>
        /// 현재 플레이어의 점수인지 확인하고 하이라이트
        /// </summary>
        private void CheckIfMyScore()
        {
            if (backgroundImage == null) return;
            
            // 현재 플레이어 이름 가져오기
            string currentPlayerName = PlayerPrefs.GetString("SavedPlayerName", "");
            
            // 이름이 일치하면 하이라이트
            if (!string.IsNullOrEmpty(currentPlayerName) && 
                player.PlayerName == currentPlayerName)
            {
                backgroundImage.color = myScoreColor;
                
                // 폰트 굵기 변경 (선택사항)
                if (rankText != null) rankText.fontStyle = FontStyles.Bold;
                if (nameText != null) nameText.fontStyle = FontStyles.Bold;
                if (scoreText != null) scoreText.fontStyle = FontStyles.Bold;
            }
            else
            {
                backgroundImage.color = normalColor;
                
                // 폰트 스타일 리셋
                if (rankText != null) rankText.fontStyle = FontStyles.Normal;
                if (nameText != null) nameText.fontStyle = FontStyles.Normal;
                if (scoreText != null) scoreText.fontStyle = FontStyles.Normal;
            }
        }
    
        /// <summary>
        /// 항목 클릭 시 처리
        /// </summary>
        private void Clicked()
        {
            if (player == null) return;
            
            // 플레이어 정보 로그 출력 (나중에 프로필 기능으로 확장 가능)
            Debug.Log($"플레이어 선택됨: {player.PlayerName} (순위: {player.Rank + 1}, 점수: {player.Score})");
            
            // TODO: 플레이어 프로필 팝업 표시 기능 추가 가능
        }
        
        /// <summary>
        /// 시간(초)을 "분:초" 형식의 문자열로 변환
        /// </summary>
        public static string FormatTime(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}