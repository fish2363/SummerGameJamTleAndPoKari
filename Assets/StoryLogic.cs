using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class StoryLogic : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    public string[] messages;
    public float typingSpeed = 0.05f;
    public PlayableDirector timeline;

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
                // ��ŵ ���: �ڷ�ƾ �ߴ��ϰ� ��ü ���� ���
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
                    if (timeline != null)
                        timeline.Play();
                }
            }
        }
    }

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
