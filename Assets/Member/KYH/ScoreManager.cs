using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("스코어 텍스트")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("스코어 바뀔때 몇턴 기다릴 건지")]
    public float scoreChange;

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
        StartCoroutine(ScoreUp());
        //if (!isShaking)
        //{
        //    isShaking = true;
        //    scoreText.GetComponent<RectTransform>().DOShakeRotation(0.2f, 50f, 5, 50f).OnComplete(() => isShaking = false);
        //}
    }

    private IEnumerator ScoreUp()
    {

        scoreText.text = $"";
        yield return new WaitForSeconds(0.1f);
        for(int i=0;i<scoreChange;i++)
        {
            scoreText.text = $"_";
            yield return new WaitForSeconds(0.2f);
            scoreText.text = $"";
            yield return new WaitForSeconds(0.2f);
            yield return null;
        }
        scoreText.text = $"{CurrentScore}";

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
