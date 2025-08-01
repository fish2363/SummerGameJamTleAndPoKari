using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GlitchTextEffect : MonoBehaviour
{
    [SerializeField] private MainmenuApi mainmenuApi;

    [Header("UI References")]
    public TMP_Text tmpText;
    public Button startButton;
    public GameObject virusBackGround;

    [Header("Scramble Settings")]
    public string targetText = "WHAT ABOUT TOMORROW?";
    public float scrambleSpeed = 0.05f;
    public float settleSpeed = 0.07f;
    public string scrambleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";

    [Header("Effect Settings")]
    public float minScale = 0.5f;
    public float maxScale = 1f;
    public float spread = 80f;

    [Header("Instance Settings")]
    [Tooltip("체인 애니메이션 및 Api 호출을 하는 주체 오브젝트인지 체크")]
    public bool isMainInstance = false;

    private bool isDelete = false;

    private void Start()
    {
        StartCoroutine(PlayScramble());
    }

    private void Update()
    {
        if (isDelete && !isMainInstance)
        {
            // 부속 인스턴스는 텍스트 페이드 아웃 처리
            tmpText.DOFade(0, 0.01f);
        }
    }

    private IEnumerator PlayScramble()
    {
        int length = targetText.Length;
        char[] result = new char[length];
        int settled = 0;
        Transform tf = tmpText.transform;

        while (settled < length)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < settled)
                    result[i] = targetText[i];
                else
                    result[i] = scrambleChars[Random.Range(0, scrambleChars.Length)];
            }

            tmpText.text = new string(result);
            yield return new WaitForSeconds(scrambleSpeed);

            if (Random.value < 0.6f)
            {
                tf.localPosition = Vector3.zero;
                Vector2 offset = Random.insideUnitCircle * spread;
                tf.localPosition += new Vector3(offset.x, offset.y, 0f);

                float randomScale = Random.Range(minScale, maxScale);
                tf.localScale = new Vector3(randomScale, randomScale, 1f);

                settled++;
            }
        }

        tmpText.text = targetText;
        isDelete = true;

        if (isMainInstance)
        {
            // 본체만 위치와 크기 원상복구 및 후속 애니메이션 처리
            tf.localPosition = Vector3.zero;
            tf.localScale = new Vector3(0.6f, 0.6f, 1f);

            yield return tf.DOScale(new Vector3(0.65f, 0.65f, 1f), 2.05f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                tf.localScale = new Vector3(0.7f, 0.7f, 1f);
                virusBackGround.SetActive(true);
                tf.DOScale(new Vector3(0.8f, 0.8f, 1f), 2f).SetEase(Ease.OutCubic);
            }).WaitForCompletion();

            yield return new WaitForSeconds(2f);

            if (mainmenuApi != null)
                mainmenuApi.Api();

            if (startButton != null)
            {
                yield return startButton.transform.DOScale(new Vector3(3f, 3f, 1f), 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
                startButton.transform.DOScale(new Vector3(2.7f, 2.7f, 1f), 0.5f).SetEase(Ease.Linear).WaitForCompletion();
            }
        }
    }
}