using Member.KYH;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    public ComboCount comboPrefab;
    public static int COMBO_CNT;

    [Header("comboBonusLine�� ����� ���������� ���ھ n��� ���ɴϴ�")]
    [SerializeField]
    private float comboBonusLine;
    private int multiple = 1;//���

    [Header("�޺� ����� ����")]
    [SerializeField] private float comboBreakTime = 5f;
    private float _currentComboValue=0f;
    private bool isComboBreak;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_currentComboValue >= 0 && isComboBreak)
            _currentComboValue -= Time.deltaTime;
        else
        {
            isComboBreak = false;
        }
    }

    public void PlusCombo(Transform parent)
    {
        if (!isComboBreak) ResetCombo();
        _currentComboValue = comboBreakTime;
        isComboBreak = true;
        ++COMBO_CNT;
        if (COMBO_CNT % comboBonusLine == 0)
            ++multiple;
        ScoreManager.Instance.Score(1 * multiple);
        ComboCount comboCount = Instantiate(comboPrefab,parent.position,Quaternion.identity);
        Debug.Log($"{comboCount.transform.position}");
        comboCount.SetText($"{COMBO_CNT}",$"x{multiple}");
        comboCount.Animate(COMBO_CNT);
    }

    public void ResetCombo()
    {
        COMBO_CNT = 0;
        multiple = 1;
        _currentComboValue = 0f;
    }
}
