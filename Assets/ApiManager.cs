using DG.Tweening;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class ApiManager : MonoBehaviour
{

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
    private float _currentTime = 0f;
    private bool isInvokingEvent = false;
    [Header("_____[마우스 조종 이벤트]_____")]
    [Header("마우스 움직일 위치 0,0 이 맨 왼쪽")]
    public Vector2[] movePath;
    [Header("마우스가 각 지점으로 이동하는데 걸리는 속도")]
    public float mouseSpeed;

    [Header("_____[마우스 고정 이벤트]_____")]
    [Header("마우스 고정 지속시간")]
    [SerializeField]
    private float mouseMoveToCenter = 3f;
    [Header("_____[창이 피해다니는 이벤트]_____")]
    [Header("마우스 감지 범위")]
    public float dodgeRadius = 200f;
    [Header("지속 시간")]
    public float moveDuration = 5f;
    [Header("창이 움직이는 속도")]
    public float moveSpeed = 10f;
    [Header("_____[속도 변경 이벤트]_____")]
    [Header("[속도 변경 지속시간]")]
    public float speedChangeDuration = 2f;
    [Header("[감소 이벤트 정도 ( 1 / n )]")]
    public int speedDown = 2;
    [Header("[증가 이벤트 정도 ( 1 * n )]")]
    public int speedUp = 2;
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


    private bool isStart;

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

        if (Input.GetKeyDown(KeyCode.L))
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        if(isInvokingEvent == false)
        {
            if (_currentTime >= invokeCooltime)
            {
                isInvokingEvent = true;
                _currentTime = 0;
                int rand = UnityEngine.Random.Range(5, 6);
                Debug.Log(rand);
                ChooseEvent(rand);
            }
            else
                _currentTime += Time.deltaTime;
        }
    }


    public void ChooseEvent(int idx)
    {
        switch(idx)
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
                break;
            case 5:
                //카메라
                RotateCamera();
                break;
            case 6:
                //속도 낮추기
                StartCoroutine(SetSpeed(speedDown));
                break;
            case 7:
                //속도 올리기
                StartCoroutine(SetSpeed(speedUp));
                break;
        }
    }

    private void RotateCamera()
    {
        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
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

    private void ShowError()
    {
        //int rand = UnityEngine.Random.Range(0,errorMassaege.Length);

        //MessageBox(hWnd, errorMassaege[rand].headerName, errorMassaege[rand].description, (uint)(0x00000001L | 0x00000030L));
        //isInvokingEvent = false;
    }

    public IEnumerator SetSpeed(int value)
    {
        if (value > 0)
            Time.timeScale = value;
        else
            Time.timeScale = 1 / Mathf.Abs(value);

        yield return new WaitForSecondsRealtime(speedChangeDuration);
        Time.timeScale = 1f;
        isInvokingEvent = false;
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
