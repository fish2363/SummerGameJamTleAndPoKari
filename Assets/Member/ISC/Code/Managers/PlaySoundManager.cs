using System;
using Ami.BroAudio;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Member.ISC.Code.Managers
{
    public class PlaySoundManager : MonoBehaviour
    {
        [SerializeField] private SoundID soundID;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider masterSlider;
        
        [SerializeField] private BroAudioType bgm;
        [SerializeField] private BroAudioType sfx;
        [SerializeField] private BroAudioType main;
        
        private void Start()
        {
            soundID.Play();
        }
        
        public void BGM(float volume)
        {
            bgmSlider.value = volume;
            BroAudio.SetVolume(bgm, volume);
        }

        public void SFX(float volume)
        {
            sfxSlider.value = volume;
            BroAudio.SetVolume(sfx, volume);
        }

        public void Master(float volume)
        {
            masterSlider.value = volume;
            BroAudio.SetVolume(main, volume);
        }
    }
}