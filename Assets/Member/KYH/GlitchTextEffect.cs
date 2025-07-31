using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GlitchTextEffect : MonoBehaviour
{
    //public TextMeshProUGUI tmpText;
    //public string targetText = "WHAT ABOUT TOMORROW?";
    //public float glitchDurationPerChar = 0.2f;
    //public float charChangeInterval = 0.05f;
    //public string glitchChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";

    //private void Start()
    //{
    //    StartCoroutine(AnimateGlitchText());
    //}

    //IEnumerator AnimateGlitchText()
    //{
    //    tmpText.text = "";

    //    for (int i = 0; i < targetText.Length; i++)
    //    {
    //        float t = 0f;
    //        char finalChar = targetText[i];

    //        while (t < glitchDurationPerChar)
    //        {
    //            t += charChangeInterval;
    //            char randomChar = glitchChars[Random.Range(0, glitchChars.Length)];
    //            tmpText.text = targetText.Substring(0, i) + randomChar + "<color=#555>▮</color>";
    //            yield return new WaitForSeconds(charChangeInterval);
    //        }

    //        tmpText.text = targetText.Substring(0, i + 1) + "<color=#555>▮</color>";
    //    }

    //    // 최종 텍스트 + 커서 유지 or 제거
    //    tmpText.text = targetText + "▮";
    //}


    public TMP_Text tmpText;
    public string targetText = "WHAT ABOUT TOMORROW?";
    public float scrambleSpeed = 0.05f;
    public float settleSpeed = 0.07f;
    public string scrambleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
    public float minScale= .5f;
    public float maxScale= 1f;
    public float spread = 80f;
    public bool isSub;
    static bool isDelete;

    private void Start()
    {
        StartCoroutine(PlayScramble());
    }
    private void Update()
    {
        if (isDelete)
            if (isSub) Destroy(tmpText.gameObject);
    }
    IEnumerator PlayScramble()
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

            // 랜덤하게 다음 글자 고정 + 이펙트
            if (Random.value < 0.6f)
            {
                // 위치 초기화 후 랜덤 오프셋
                tf.localPosition = Vector3.zero;
                Vector2 offset = Random.insideUnitCircle * spread;
                tf.localPosition += new Vector3(offset.x, offset.y, 0f);

                // 스케일 랜덤
                float randomScale = Random.Range(minScale, maxScale);
                tf.localScale = new Vector3(randomScale, randomScale, 1f);

                settled++;
            }
        }

        tmpText.text = targetText;
        isDelete = true;
        // 원래 위치와 크기로 되돌리기
        tf.localPosition = Vector3.zero;
        tf.localScale = new Vector3(0.6f, 0.6f, 1f);
        tf.DOScale(new Vector3(0.7f, 0.7f, 1f), 3f).SetEase(Ease.OutCubic);
    }

}
