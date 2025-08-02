using DG.Tweening;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Ami.BroAudio;
using UnityEngine;
using TMPro;
using Member.CUH.Code.Enemies;
using UnityEngine.UI;
using Member.KDH.Code.Bullet;
using UnityEngine.Rendering.Universal;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance;
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] private static extern bool GetCursorPos(out POINT lpPoint);
    [DllImport("user32.dll")] private static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")] private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    [DllImport("user32.dll")] private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    [DllImport("user32.dll",
        CharSet = CharSet.Unicode)] private static extern int MessageBox(IntPtr hwnd, string lpText, string lpCaption, uint flags);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int X; public int Y; }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public System.IntPtr dwExtraInfo;
    }

    const uint INPUT_MOUSE = 0;
    const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    const uint MOUSEEVENTF_LEFTUP = 0x0004;
    const int SW_MINIMIZE = 6;

    private IntPtr hWnd;
    private Vector2 currentPos;
    private Vector2 targetPos;
    private Vector2 windowSize;

    [Header("이벤트 발생 쿨타임")]
    [SerializeField] private float invokeCooltime = 15f;
    [Header("api가 강화되는 강화 적 수")]
    [SerializeField] private float overClockCnt = 2f;

    [Header("-----[이벤트 발생 알림]-----")]
    [SerializeField] private GameObject alim;
    [SerializeField] private TMP_Text alimText;
    [SerializeField] private GameObject overClockText;
    [SerializeField] private GameObject overClockAlim;
    [Header("오버클럭 알람 깜빡 대기 시간")]
    [SerializeField] private float overClockAlimDuration = 0.5f;



    [Header("처음 깜빡임 간격")]
    public float startInterval = 1.0f;
    [Header("마지막 깜빡임 간격")]
    public float endInterval = 0.1f;  
    [Header("깜빡이기 전 대기 시간")]
    public float stopDuration = 2f;  
    [Header("전체 지속 시간")]
    public float totalDuration = 5f;  
    private float appearDuration = 0.4f;
    private Ease easeType = Ease.OutBack; // 뽀용~ 느낌
    private Coroutine flashCoroutine;
    private float multiplyGreatEnemyCnt;

    private float _currentTime = 0f;
    private bool isInvokingEvent = false;
    [Header("_____[마우스 조종 이벤트]_____")]
    [Header("마우스 움직일 위치 0,0 이 맨 왼쪽")]
    public Vector2[] movePath;
    [Header("마우스가 각 지점으로 이동하는데 걸리는 속도")]
    public float mouseSpeed;
    [Header("경고 메세지")]
    public string alimEventText_mouse;

    [Header("_____[마우스 고정 이벤트]_____")]
    [Header("마우스 고정 지속시간")]
    [SerializeField]
    private float mouseMoveToCenter = 3f;
    [Header("경고 메세지")]
    public string alimEventText_fixedMouseMiddlePoint;
    [Header("_____[창이 피해다니는 이벤트]_____")]
    [Header("마우스 감지 범위")]
    public float dodgeRadius = 200f;
    [Header("지속 시간")]
    public float moveDuration = 5f;
    [Header("창이 움직이는 속도")]
    public float moveSpeed = 10f;
    [Header("경고 메세지")]
    public string alimEventText_runawayScreen;
    [Header("_____[속도 변경 이벤트]_____")]
    [Header("[속도 변경 지속시간]")]
    public float speedChangeDuration = 2f;
    public UnityEngine.Rendering.Volume volume;
    private ColorAdjustments colorAdjust;
    [Header("경고 메세지")]
    public string alimEventText_upSpeed;
    [Header("경고 메세지")]
    public string alimEventText_downSpeed;
    [Header("_____[카메라 회전 이벤트]_____")]
    [Header("회전 Ease")]
    public Ease rotationEase;
    [Header("회전 지속시간")]
    public float rotateDuration = 2f;
    [Header("회전되는 시간")]
    public float rotateSpeed = 2f;
    [Header("흔들기 지속시간")]
    public float shakeDuration = 1f;
    private float shakeMagnitude = 10f;
    private Camera camera;
    [Header("경고 메세지")]
    public string alimEventText_rotateCamera;
    [Header("_____[사진 띄우기 이벤트]_____")]
    [Header("Resource 폴더 내에 사진 이름")]
    public string fileName = "ItsMine"; // Resources 폴더 내부 경로
    [Header("경고 메세지")]
    public string alimEventText_photo;

    [Header("_____[창 최소화 이벤트]_____")]
    [Header("경고 메세지")]
    public string alimEventText_Minimalize;

    [SerializeField] private SoundID warningSound;
    [SerializeField] private SoundID slowSound;
    [SerializeField] private SoundID timeAccelerationSound;
    [SerializeField] private SoundID screenRunningSound;
    [SerializeField] private SoundID cameraRotationSound;
    
    private bool isStart;
    private int randIdx;
    public bool IsBoss { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
        camera = Camera.main;
        hWnd = GetActiveWindow();

        if (GetWindowRect(hWnd, out RECT rect))
        {
            currentPos = new Vector2(rect.Left, rect.Top);
            windowSize = new Vector2(rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        isStart = true;
    }

    void Update()
    {
        if (!isStart) return;

        if (EnemyManager.Instance.OverClockEnemyCount >= overClockCnt)
        {
            multiplyGreatEnemyCnt = EnemyManager.Instance.OverClockEnemyCount / 3;
            StartCoroutine(OverClockAlimRoutine());
        }
        else
            multiplyGreatEnemyCnt = 1f;
        if (!IsBoss)
        {
            if (isInvokingEvent == false)
            {
                if (_currentTime >= invokeCooltime)
                {
                    isInvokingEvent = true;
                    _currentTime = 0;
                    Appear();
                }
                else
                {
                    _currentTime += Time.deltaTime * multiplyGreatEnemyCnt;
                }
            }

            foregroundImage.fillAmount = Mathf.Clamp01(_currentTime / invokeCooltime);

            if (backgroundImage.fillAmount > Mathf.Clamp01(_currentTime / invokeCooltime))
            {
                backgroundImage.fillAmount -= Time.deltaTime * delaySpeed;
                if (backgroundImage.fillAmount < Mathf.Clamp01(_currentTime / invokeCooltime))
                    backgroundImage.fillAmount = Mathf.Clamp01(_currentTime / invokeCooltime);
            }
            else
            {
                backgroundImage.fillAmount = Mathf.Clamp01(_currentTime / invokeCooltime);
            }
        }
    }
    public enum Direction { Top, Bottom, Left, Right }
    void MoveToEdge(Direction dir)
    {
        GetWindowRect(hWnd, out RECT rect);

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        int x = rect.Left;
        int y = rect.Top;

        switch (dir)
        {
            case Direction.Top:
                y = 0;
                break;
            case Direction.Bottom:
                y = screenRect.Bottom - height;
                break;
            case Direction.Left:
                x = 0;
                break;
            case Direction.Right:
                x = screenRect.Right - width;
                break;
        }

        MoveWindow(windowHandle, x, y, width, height, true);
    }

    public Image foregroundImage;    
    public Image backgroundImage;
    public TextMeshProUGUI _warningText;
    public float delaySpeed = 0.5f;  

    public void MinusGageValue(int value)
    {
        _currentTime = Mathf.Clamp(_currentTime - value,0,invokeCooltime);
    }
    private IEnumerator OverClockAlimRoutine()
    {
        overClockAlim.SetActive(true);
        yield return new WaitForSecondsRealtime(overClockAlimDuration);
        overClockAlim.SetActive(false);
        if (EnemyManager.Instance.OverClockEnemyCount >= overClockCnt)
            StartCoroutine(OverClockAlimRoutine());
    }



    public void Appear()
    {
        if(EnemyManager.Instance.OverClockEnemyCount >= overClockCnt)
        {

            overClockText.SetActive(true);
        }

        randIdx = UnityEngine.Random.Range(6, 8);
        UnityEngine.Debug.Log(randIdx);
        ChangeTextEvent(randIdx);

        alim.transform.localScale = new Vector3(1.620305f, 0f, 1f); // 아래서 시작
        alim.transform.DOScaleY(1.620305f, appearDuration).SetEase(easeType);

        warningSound.Play();
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    public void ChangeTextEvent(int idx)
    {
        switch (idx)
        {
            case 0:
                alimText.text = alimEventText_mouse;
                break;
            case 1:
                alimText.text = alimEventText_runawayScreen;
                break;
            case 2:
                alimText.text = alimEventText_fixedMouseMiddlePoint;
                break;
            case 3:
                alimText.text = alimEventText_Minimalize;
                break;
            case 4:
                alimText.text = alimEventText_photo;
                break;
            case 5:
                alimText.text = alimEventText_rotateCamera;
                break;
            case 6:
                //속도 낮추기
                alimText.text = alimEventText_downSpeed;
                break;
            case 7:
                //속도 올리기
                alimText.text = alimEventText_upSpeed;
                break;
        }
    }

    IEnumerator FlashRoutine()
    {
        yield return new WaitForSecondsRealtime(stopDuration);
        
        float elapsed = 0f;
        bool visible = true;
        while (elapsed < totalDuration)
        {
            float t = elapsed / totalDuration;
            float currentInterval = Mathf.Lerp(startInterval, endInterval, t);

            alim.SetActive(visible);
            visible = !visible;

            yield return new WaitForSeconds(currentInterval);
            elapsed += currentInterval;
        }
        alim.SetActive(false);
        alim.transform.localScale = new Vector3(1f, 0f, 1f); // 아래서 시작
        ChooseEvent(randIdx);
    }

    public void ChooseEvent(int idx)
    { 
        switch (idx)
        {
            case 0:
                StartCoroutine(MoveMouseSequence());
                break;
            case 1:
                StartCoroutine(MoveScreenRoutine());
                break;

            case 2:
                StartCoroutine(FixMouseToCenter(mouseMoveToCenter));
                break;
            case 3:
                Minimize();
                break;
            case 4:
                //창 띄우기
                isInvokingEvent = false;

                Texture2D texture = Resources.Load<Texture2D>("MyImages/ItsMine");
                if (texture == null)
                {
                    UnityEngine.Debug.LogError("Resources에서 이미지를 찾을 수 없습니다.");
                    return;
                }

                string destPath = Path.Combine(Application.persistentDataPath, "ItsMine.png");
                byte[] pngBytes = texture.EncodeToPNG();
                File.WriteAllBytes(destPath, pngBytes);

                Process.Start(new ProcessStartInfo(destPath) { UseShellExecute = true });
                break;
            case 5:
                //카메라
                RotateCamera();
                break;
            case 6:
                //속도 낮추기
                StartCoroutine(SetSpeed(-1));
                break;
            case 7:
                //속도 올리기
                StartCoroutine(SetSpeed(1));
                break;
        }
        overClockText.SetActive(false);
    }

    private void RotateCamera()
    {
        cameraRotationSound.Play();
        StartCoroutine(RotateAndShakeRoutine());
    }

    IEnumerator RotateAndShakeRoutine()
    {
        GetWindowRect(hWnd, out RECT rect);
        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        float timer = 0f;
        Vector2 originalPos = new Vector2(rect.Left, rect.Top);

        while (timer < shakeDuration)
        {
            float offsetX = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);

            MoveWindow(hWnd, (int)(originalPos.x + offsetX), (int)(originalPos.y + offsetY), width, height, true);
            timer += Time.deltaTime;
            yield return null;
        }

        // 원래 위치로 복구
        MoveWindow(hWnd, (int)originalPos.x, (int)originalPos.y, width, height, true);
        camera.transform.DORotate(new Vector3(0, 0, 180), rotateSpeed).SetEase(rotationEase);
        DOVirtual.DelayedCall(rotateDuration, () =>
        {
            camera.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.Linear);
            isInvokingEvent = false;
        });
    }

    public void ShakeScreen()
    {
        StartCoroutine(ScreenShakeRoutine());
    }

    IEnumerator ScreenShakeRoutine()
    {
        GetWindowRect(hWnd, out RECT rect);
        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        float timer = 0f;
        Vector2 originalPos = new Vector2(rect.Left, rect.Top);

        while (timer < shakeDuration)
        {
            float offsetX = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);

            MoveWindow(hWnd, (int)(originalPos.x + offsetX), (int)(originalPos.y + offsetY), width, height, true);
            timer += Time.deltaTime;
            yield return null;
        }

        // 원래 위치로 복구
        MoveWindow(hWnd, (int)originalPos.x, (int)originalPos.y, width, height, true);
    }

    private void ShowError()
    {
        //int rand = UnityEngine.Random.Range(0,errorMassaege.Length);

        //MessageBox(hWnd, errorMassaege[rand].headerName, errorMassaege[rand].description, (uint)(0x00000001L | 0x00000030L));
        //isInvokingEvent = false;
    }

    public IEnumerator SetSpeed(int value)
    {
        if (volume.profile.TryGet(out colorAdjust))
        {
            DOTween.To(() => colorAdjust.postExposure.value, x => colorAdjust.postExposure.value = x,-76f,2f).SetEase(Ease.OutCubic);
        }

        if (value > 0)
        {
            timeAccelerationSound.Play();
            Bullet.isFaster = true;
        }
        else
        {
            slowSound.Play();
            Bullet.isSlowy = true;
        }

        yield return new WaitForSecondsRealtime(speedChangeDuration);
        Bullet.isFaster= false;
        Bullet.isSlowy = false;
        isInvokingEvent = false;
        DOTween.To(() => colorAdjust.postExposure.value, x => colorAdjust.postExposure.value = x, 0f, 0.2f).SetEase(Ease.OutCubic);
    }

    #region 창 움직이기

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
                screenRunningSound.Play();
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
        isInvokingEvent = false;
    }
    #endregion
    #region 마우스 고정
    IEnumerator FixMouseToCenter(float duration)
    {
        float elapsed = 0f;

        int centerX = Screen.currentResolution.width / 2;
        int centerY = Screen.currentResolution.height / 2;

        while (elapsed < duration)
        {
            SetCursorPos(centerX, centerY);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isInvokingEvent = false;

    }
    #endregion
    #region 마우스 움직이기
    IEnumerator MoveMouseSequence()
    {
        POINT p;
        GetCursorPos(out p);
        Vector2 currentPos = new Vector2(p.X, p.Y);

        if (movePath.Length > 0)
        {
            yield return MoveMouseSmoothly(currentPos, movePath[0], mouseSpeed);
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < movePath.Length - 1; i++)
        {
            Vector2 from = movePath[i];
            Vector2 to = movePath[i + 1];

            yield return MoveMouseSmoothly(from, to, mouseSpeed);
            yield return new WaitForSeconds(0.1f);
        }
        isInvokingEvent = false;

    }

    IEnumerator MoveMouseSmoothly(Vector2 start, Vector2 end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float x = Mathf.Lerp(start.x, end.x, t);
            float y = Mathf.Lerp(start.y, end.y, t);

            SetCursorPos((int)x, (int)y);
            yield return null;
        }

        SetCursorPos((int)end.x, (int)end.y);
    }

    public static void Click()
    {
        INPUT[] inputs = new INPUT[2];

        inputs[0].type = INPUT_MOUSE;
        inputs[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;

        inputs[1].type = INPUT_MOUSE;
        inputs[1].mi.dwFlags = MOUSEEVENTF_LEFTUP;

        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }
    #endregion
    #region 마우스 움직이기
    public void Minimize()
    {
        IntPtr handle = GetActiveWindow(); // Unity 실행창 핸들 얻기
        ShowWindow(handle, SW_MINIMIZE);   // 최소화 명령
        isInvokingEvent = false;
    }
    #endregion

}
