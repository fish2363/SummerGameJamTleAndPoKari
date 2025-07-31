using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("시간 텍스트")]
    [SerializeField] private TextMeshProUGUI timeText;

    [field:SerializeField] public float CurrentTime => _floatTime;
    private float _floatTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        timeText.text = (_floatTime += Time.deltaTime).ToString("F2");
    }
}
