using System;
using Member.ISC.Code.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Member.ISC.Code.UI
{
    public class DashCoolUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private PlayerMoveCompo playerMoveCompo;
        [SerializeField] private Canvas canvas;
        
        private bool _isActive = true;
        private bool _rotateFreeze = true;

        private void Start()
        {
            slider.maxValue = playerMoveCompo.DashCool;
            slider.value = slider.maxValue;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_isActive)
                SliderUpdate();
            
        }

        private void LateUpdate()
        {
            if (_rotateFreeze)
                canvas.transform.rotation = Quaternion.identity;
        }

        public void SliderUpdate()
        {
            float value = playerMoveCompo.DashCool - playerMoveCompo.CurrentDashTime;
            
            slider.value = Mathf.Clamp(value, 0, playerMoveCompo.DashCool);
            
            if (playerMoveCompo.CanDash)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _isActive = true;
        }

        private void OnDisable()
        {
            _isActive = false;
        }
    }
}