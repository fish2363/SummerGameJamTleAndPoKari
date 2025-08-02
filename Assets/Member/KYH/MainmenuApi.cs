using DG.Tweening;
using Leaderboard.Scripts.Menu;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuApi : MonoBehaviour
{
    [Header("_____[창이 피해다니는 이벤트]_____")]
    [Header("마우스 감지 범위")]
    public float dodgeRadius = 200f;
    [Header("지속 시간")]
    public float moveDuration = 5f;
    [Header("창이 움직이는 속도")]
    public float moveSpeed = 10f;

    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] private static extern bool GetCursorPos(out POINT lpPoint);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")] private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int X; public int Y; }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject fakeTitle;
    [SerializeField] private Transform titlePos;
    [SerializeField] private TMP_Text pressStartButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Image startPanel;
    [SerializeField] private GameObject[] glitchTexts;

    public Volume volume;
    private ChromaticAberration chromatic;

    private IntPtr hWnd;
    private Vector2 currentPos;
    private Vector2 windowSize;
    private Vector2 targetPos;
    public GameObject virusBackGround;
    public CanvasGroup tutorialCanvas;
    public CanvasGroup gameCanvas;
    public CanvasGroup LBCanvas;
    public CanvasGroup otherCanvas;

    public Button[] buttons;
    public Transform[] buttonPos;
    public float moveButtonDuration = 0.5f;
    public Ease moveEase = Ease.OutCubic;
    private void Awake()
    {
        hWnd = GetActiveWindow();
        if (GetWindowRect(hWnd, out RECT rect))
        {
            currentPos = new Vector2(rect.Left, rect.Top);
            windowSize = new Vector2(rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        string cutScene = PlayerPrefs.GetString("SKIP", "NO");
        if (cutScene == "NO")
        {
            PlayerPrefs.SetString("SKIP", "YES");
            PlayerPrefs.Save();
            for(int i=0;i<glitchTexts.Length;i++)
            {
                glitchTexts[i].gameObject.SetActive(true);
            }
        }
        else
        {
            MenuManager manager = FindAnyObjectByType<MenuManager>();
            manager.ClicktoStartClientService();
            virusBackGround.SetActive(true);
            fakeTitle.SetActive(true);
            ClickStart();
        }
    }
    public void Api()
    {
        StartCoroutine(MoveScreenRoutine());
    }

    IEnumerator MoveScreenRoutine()
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            if (!GetWindowRect(hWnd, out RECT rect)) break;

            currentPos = new Vector2(rect.Left, rect.Top);
            windowSize = new Vector2(rect.Right - rect.Left, rect.Bottom - rect.Top);

            if (!GetCursorPos(out POINT cursor)) break;

            Vector2 mousePos = new Vector2(cursor.X, cursor.Y);
            float dist = Vector2.Distance(mousePos, currentPos + windowSize / 2f);

            if (dist < dodgeRadius)
            {
                // 도망갈 방향은 마우스에서 멀어지는 방향
                Vector2 dir = (currentPos + windowSize / 2f - mousePos).normalized;
                Vector2 newTarget = currentPos + dir * dodgeRadius;

                // 화면 경계 안쪽으로 제한
                newTarget.x = Mathf.Clamp(newTarget.x, 0, Screen.currentResolution.width - windowSize.x);
                newTarget.y = Mathf.Clamp(newTarget.y, 0, Screen.currentResolution.height - windowSize.y);

                targetPos = newTarget;
            }

            // 부드럽게 이동
            currentPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * moveSpeed);
            MoveWindow(hWnd, (int)currentPos.x, (int)currentPos.y, (int)windowSize.x, (int)windowSize.y, true);
            elapsed += Time.deltaTime;
            yield return null;
        }
        pressStartButton.DOColor(Color.red,0.2f);
        startButton.enabled = true;
    }

    public void ClickStart()
    {
        //if (volume.profile.TryGet(out chromatic))
        //{
        //    Debug.Log("시작");
        //    StartCoroutine(FadeChromaticAberration());
        //}
        pressStartButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        title.transform.DOMove(titlePos.position, moveButtonDuration).SetEase(moveEase);
        title.transform.DOScale(new Vector3(1.3f, 1.3f), moveButtonDuration).SetEase(moveEase); //크기도 같이 제어
        StartCoroutine(ShowButtonsSequentially());
    }

    private IEnumerator ShowButtonsSequentially()
    {
        for (int i = 0; i < buttons.Length && i < buttonPos.Length; i++)
        {
            Button btn = buttons[i];
            Transform targetPos = buttonPos[i];
            btn.enabled = true;
            yield return btn.transform.DOMove(targetPos.position, moveButtonDuration).SetEase(moveEase).WaitForCompletion();
        }
    }
    IEnumerator FadeChromaticAberration()
    {
        float duration = 1f;
        float elapsed = 0f;
        float startValue = 1f;
        float endValue = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            chromatic.intensity.value = Mathf.Lerp(startValue, endValue, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        chromatic.intensity.value = endValue;
    }
    public void StartGame()
    {
        startPanel.raycastTarget = true;
        startPanel.DOFade(1f, 1f).OnComplete(() =>
        { SceneManager.LoadScene("KYH"); }
        );
       // StartCoroutine(FadeChromaticAberration());
    }
    public void Tutorial()
    {
        tutorialCanvas.DOFade(1f, 0.5f);
        tutorialCanvas.blocksRaycasts = true;
        tutorialCanvas.interactable = true;
        AllGameUISetActive(false);

        LBSet();
    }

    private void LBSet()
    {
        LBCanvas.DOFade(0f, 0.5f);
        LBCanvas.blocksRaycasts = false;
        LBCanvas.interactable = false;
    }

    public void AllGameUISetActive(bool isValue)
    {
        int power = isValue ? 1 : 0;
        gameCanvas.DOFade(power, 0.5f);
        gameCanvas.blocksRaycasts = isValue;
        gameCanvas.interactable = isValue;
        otherCanvas.DOFade(power, 0.5f);
        otherCanvas.blocksRaycasts = isValue;
        otherCanvas.interactable = isValue;
        
    }
    private bool isOn=true;
    public CanvasGroup esc;
    public void AllGameUISetActiveESC()
    {
        isOn = !isOn;
        int power = isOn ? 1 : 0;
        gameCanvas.DOFade(power, 0.5f);
        gameCanvas.blocksRaycasts = isOn;
        gameCanvas.interactable = isOn;
        otherCanvas.DOFade(power, 0.5f);
        otherCanvas.blocksRaycasts = isOn;
        otherCanvas.interactable = isOn;
        lbButton.gameObject.SetActive(isOn);

        int powerSS = isOn ? 0 : 1;
        esc.DOFade(powerSS, 0.5f);
        esc.blocksRaycasts = !isOn;
        esc.interactable = !isOn;
    }
    public bool isLB;
    public void AllGameUISetActive()
    {
        isLB = !isLB;
        gameCanvas.DOFade(0f, 0.5f);
        gameCanvas.blocksRaycasts = isLB;
        gameCanvas.interactable = isLB;
        otherCanvas.DOFade(0f, 0.5f);
        otherCanvas.blocksRaycasts =isLB;
        otherCanvas.interactable = isLB;
        lbButton.SetActive(!isLB);
    }
    public GameObject lbButton;

    public void TutorialCancel()
    {
        tutorialCanvas.DOFade(0f, 0.5f);
        tutorialCanvas.blocksRaycasts =false;
        tutorialCanvas.interactable = false;

        AllGameUISetActive(true);
        LBCanvas.DOFade(1f, 0.5f);
        LBCanvas.blocksRaycasts = true;
        LBCanvas.interactable = true;
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
