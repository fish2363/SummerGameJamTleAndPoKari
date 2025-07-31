using UnityEngine;
using UnityEngine.UI;

public class RainbowGradientImage : Image
{
    private Color bottomColor = new Color(0.1f, 0.1f, 0.2f);

    [SerializeField, Range(1f, 60f)]
    private float cycleDuration = 10f; // �� ���� ���� �� �ɸ��� �ð� (��)
    private Color currentTopColor;
    public Color CurrentTopColor => currentTopColor;
    private Color GetRainbowColor(float time)
    {
        float speed = 1f / cycleDuration;
        float hue = Mathf.Repeat(time * speed, 1f);
        return Color.HSVToRGB(hue, 0.4f, 0.9f); // ���� ��������
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);

        if (!Application.isPlaying)
            return;

        float currentTime = Time.time;
        Color topColor = GetRainbowColor(currentTime);

        currentTopColor = topColor; // **���� �߰�!**

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            UIVertex vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);

            float normalizedY = Mathf.InverseLerp(rectTransform.rect.yMin, rectTransform.rect.yMax, vertex.position.y);

            vertex.color = Color.Lerp(bottomColor, topColor, normalizedY);

            vh.SetUIVertex(vertex, i);
        }
    }

    void Update()
    {
        // �÷��� �߿��� �������� ����
        if (Application.isPlaying)
        {
            SetVerticesDirty(); // �� ������ �ٽ� �׸����� ��û
        }
    }
}