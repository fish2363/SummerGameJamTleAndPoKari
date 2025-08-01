using TMPro;
using UnityEngine;

public class ComboCount : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text percentText;

    public void SetText(string value,string comboPercent)
    {
        text.text = value;
        percentText.text = comboPercent;
    }
}
