using Ami.BroAudio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class ESCManager : MonoBehaviour
{
    private bool isOn;
    private bool isEscOn;
    [SerializeField] CanvasGroup escCanvas;
    [SerializeField] CanvasGroup chooseCanvas;

    [SerializeField] private BroAudioType _bgm;
    [SerializeField] private BroAudioType _sfx;
    [SerializeField] private BroAudioType _main;

    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;


    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            PressEscape();
    }

    public void ESC()
    {
        if(!isEscOn)
            ChoosePanel();
        isEscOn = !isEscOn;
        float force = isEscOn ? 1 : 0;
        escCanvas.DOFade(force, 0.2f);
        escCanvas.blocksRaycasts = isEscOn;
        escCanvas.interactable = isEscOn;
        Time.timeScale = isEscOn ? 0f : 1f;
    }

    private void PressEscape()
    {
        if (isEscOn)
            ESC();
        else
            ChoosePanel();
    }

    public void ChoosePanel()
    {
        isOn = !isOn;
        float force = isOn ? 1 : 0;
        chooseCanvas.DOFade(force, 0.2f);
        chooseCanvas.blocksRaycasts = isOn;
        chooseCanvas.interactable = isOn;
        Time.timeScale = isOn ? 0f : 1f;
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
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 에디터 모드에서 실행 중이면 Play 모드를 끔
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();
#endif
    }
}
