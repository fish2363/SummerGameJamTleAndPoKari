using UnityEngine;
using UnityEngine.UI;

public class ImageColorChange : MonoBehaviour
{
    public RainbowGradientImage rainbowImage;
    public Image images;
    private void Start()
    {
        rainbowImage = FindAnyObjectByType<RainbowGradientImage>();
    }
    void Update()
    {
        Color baseColor = rainbowImage.CurrentTopColor;

        // HSV ���� ���� ���
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        h = (h + 0.5f) % 1f; // 180�� ����
        Color complementary = Color.HSVToRGB(h, s, v);

        images.color = complementary;
    }
}
