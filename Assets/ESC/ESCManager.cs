using Ami.BroAudio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ESCManager : MonoBehaviour
{
    private bool isChoosePanelOn;
    private bool isEscPanelOn;

    [SerializeField] private CanvasGroup escCanvas;     // 설정창 (EscPanel)
    [SerializeField] private CanvasGroup chooseCanvas;  // ESC 눌렀을 때 뜨는 첫 패널

    [SerializeField] private BroAudioType _bgm;
    [SerializeField] private BroAudioType _sfx;
    [SerializeField] private BroAudioType _main;

    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscapeKey();
        }
    }

    private void HandleEscapeKey()
    {
        if (isChoosePanelOn)
        {
            CloseChoosePanel();
            ResumeGame();
        }
        else if (isEscPanelOn)
        {
            CloseEscPanel();
            ResumeGame();
        }
        else
        {
            OpenChoosePanel();
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    private void OpenChoosePanel()
    {
        isChoosePanelOn = true;

        // 패널 알파 및 인터랙션 설정
        chooseCanvas.alpha = 1;
        chooseCanvas.interactable = true;
        chooseCanvas.blocksRaycasts = true;

        // 애니메이션용 RectTransform 가져오기
        RectTransform rect = chooseCanvas.GetComponent<RectTransform>();
        rect.localScale = new Vector3(0f, 1f, 1f); // X만 0으로 시작

        // DOTween 애니메이션 (UnscaledTime으로 재생)
        rect.DOScaleX(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void CloseChoosePanel()
    {
        isChoosePanelOn = false;
        chooseCanvas.alpha = 0;
        chooseCanvas.interactable = false;
        chooseCanvas.blocksRaycasts = false;
    }

    private void OpenEscPanel()
    {
        isEscPanelOn = true;
        escCanvas.alpha = 1;
        escCanvas.interactable = true;
        escCanvas.blocksRaycasts = true;
    }

    private void CloseEscPanel()
    {
        isEscPanelOn = false;
        escCanvas.alpha = 0;
        escCanvas.interactable = false;
        escCanvas.blocksRaycasts = false;
    }

    public void OnClickContinue()
    {
        CloseChoosePanel();
        ResumeGame();
    }

    public void OnClickSettings()
    {
        CloseChoosePanel();
        OpenEscPanel();
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void BGM(float volume)
    {
        _bgmSlider.value = volume;
        BroAudio.SetVolume(_bgm, volume);
    }

    public void SFX(float volume)
    {
        _sfxSlider.value = volume;
        BroAudio.SetVolume(_sfx, volume);
    }

    public void Master(float volume)
    {
        _masterSlider.value = volume;
        BroAudio.SetVolume(_main, volume);
    }
}