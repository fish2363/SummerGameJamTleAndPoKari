using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class StoryLogic : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    public string[] messages;
    public float typingSpeed = 0.05f;

    private int index = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentMessage = "";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // 스킵 기능: 코루틴 중단하고 전체 문장 출력
                StopCoroutine(typingCoroutine);
                targetText.text = currentMessage;
                isTyping = false;
            }
            else
            {
                if (index < messages.Length)
                {
                    typingCoroutine = StartCoroutine(TypeText(messages[index]));
                    currentMessage = messages[index];
                    index++;
                }
                else
                {
                    //if (Answer(appDescript, appName) == 1)
                    //{
                    //    Process.Start("https://ggm-h.goeay.kr/ggm-h/main.do");
                    //    isExecute = false;
                    //}
                    //else
                    //{
                    //    isExecute = false;
                    //}
                }
            }
        }
    }

    //private int Answer(string appName, string appDescript)
    //{
    //    int answer;
    //    answer = MessageBox(GetWindowHandle(), appName, appDescript, (uint)(0x00000001L | 0x00000030L));
    //    return answer;
    //}

    IEnumerator TypeText(string message)
    {
        isTyping = true;
        targetText.text = "";

        foreach (char letter in message)
        {
            targetText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
