using DG.Tweening;
using Leaderboard.Scripts.Menu;
using Leaderboard.Scripts.Tools;
using TMPro;
using UnityEngine;

namespace Member.KYH
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;

        [Header("스코어 텍스트")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        [field: SerializeField]
        public int CurrentScore { get; set; }

        private bool isShaking;
        private float gameStartTime;

        private void Awake()
        {
            Instance = this;
            gameStartTime = Time.time;
        }
    
        public void Score(int score)
        {
            CurrentScore += score;
            SetText();
        }

        void SetText()
        {
            scoreText.text = $"{CurrentScore}";
            if (!isShaking)
            {
                isShaking = true;
                scoreText.GetComponent<RectTransform>().DOShakeRotation(0.2f, 50f, 5, 50f).OnComplete(() => isShaking = false);
            }
        }

        public void SetHighScore()
        {
            int finalScore = CurrentScore;
            float playTime = Time.time - gameStartTime;
        
            scoreText.text = $"{finalScore}";
        
            int previousHighScore = PlayerPrefs.GetInt("score", 0);
            highScoreText.text = $"{previousHighScore}";

            if (finalScore > previousHighScore)
            {
                PlayerPrefs.SetInt("score", finalScore);
                PlayerPrefs.Save();
                Debug.Log($"새로운 최고점수 달성: {finalScore}점");
            }
        
            ShowGameOverRanking(finalScore, playTime);
        }
    
        private void ShowGameOverRanking(int score, float playTime)
        {
            GameOverRankingMenu rankingMenu = (GameOverRankingMenu)PanelManager.GetSingleton("gameOverRanking");
            if (rankingMenu != null)
            {
                rankingMenu.ShowRankingAfterScore(score, playTime);
                Debug.Log($"게임 오버 랭킹 표시: {score}점, 플레이 타임: {playTime:F1}초");
            }
            else
            {
                Debug.LogError("GameOverRankingMenu를 찾을 수 없습니다. Panel ID를 'gameOverRanking'으로 설정했는지 확인하세요.");
            }
        }
    
        public float GetCurrentPlayTime()
        {
            return Time.time - gameStartTime;
        }
    
        public void ResetGame()
        {
            CurrentScore = 0;
            gameStartTime = Time.time;
            SetText();
            Debug.Log("게임 리셋됨");
        }
    }
}