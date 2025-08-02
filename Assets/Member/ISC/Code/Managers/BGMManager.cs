using System;
using Ami.BroAudio;
using UnityEngine;

namespace Member.ISC.Code.Managers
{
    public class BGMManager : MonoBehaviour
    {
        [SerializeField] private SoundID soundID;

        private void Start()
        {
            soundID.Play();
        }
    }
}