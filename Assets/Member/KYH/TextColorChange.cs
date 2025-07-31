using TMPro;
using UnityEngine;

public class TextColorChange : MonoBehaviour
{
    public RainbowGradientImage rainbowImage;
    public TMP_Text[] tmpTexts;

    void Update()
    {
        if (rainbowImage == null || tmpTexts == null)
            return;

        Color baseColor = rainbowImage.CurrentTopColor;

        // HSV ���� ���� ���
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        h = (h + 0.5f) % 1f; // 180�� ����
        Color complementary = Color.HSVToRGB(h, s, v);

        foreach (var tmp in tmpTexts)
        {
            if (tmp != null)
                tmp.color = complementary;
        }
    }
}