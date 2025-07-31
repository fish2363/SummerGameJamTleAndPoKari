using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("���ھ� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [field: SerializeField]
    public int CurrentScore { get; set; }

    private bool isShaking;

    private void Awake()
    {
        Instance = this;
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
            scoreText.GetComponent<RectTransform>().DOShakeRotation(0.2f, 20f, 10, 10f).OnComplete(() => isShaking = false);
        }
    }

    public void SetHighScore()
    {
        int levelNum = CurrentScore;
        scoreText.text = $"{levelNum}";
        highScoreText.text = $"{PlayerPrefs.GetInt("score", levelNum)}";

        if (levelNum > PlayerPrefs.GetInt("score", 0))
            PlayerPrefs.SetInt("score", levelNum);
    }
}
