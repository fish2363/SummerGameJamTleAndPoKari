using DG.Tweening;
using UnityEngine;

public class TestObj : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float duration = 1f;

    void Start()
    {
        // ó�� ��ġ�� A�� ����
        transform.position = pointA.position;

        // A �� B �� A �� ���� �ݺ�
        transform.DOMove(pointB.position, duration)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }
}
