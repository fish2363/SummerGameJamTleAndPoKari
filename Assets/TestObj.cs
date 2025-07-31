using DG.Tweening;
using UnityEngine;

public class TestObj : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float duration = 1f;

    void Start()
    {
        // 처음 위치를 A로 맞춤
        transform.position = pointA.position;

        // A → B → A 를 무한 반복
        transform.DOMove(pointB.position, duration)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }
}
