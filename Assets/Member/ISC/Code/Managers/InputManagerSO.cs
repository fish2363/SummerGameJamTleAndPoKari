using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.ISC.Code.Managers
{
    [CreateAssetMenu(fileName = "InputManager", menuName = "SO/InputManager", order = 0)]
    public class InputManagerSO : ScriptableObject, Contorls.IPlayerActions
    
    {
        private Contorls _controls;
        
        public Action OnAttackPressed;
        public Action OnAttackCanceled;
        public Action OnDashPressed;
        
        public Vector2 MovementKey { get; private set; }
        public Vector2 MousePos { get; private set; }
        
        
        private void OnEnable()
        { 
            _controls ??= new Contorls();
            
            _controls.Player.SetCallbacks(this);
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            MovementKey = dir;
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAttackPressed?.Invoke();

            if (context.canceled)
                OnAttackCanceled?.Invoke();
        }

        public void OnPos(InputAction.CallbackContext context)
        {
            Vector2 pos = context.ReadValue<Vector2>();
            MousePos = Camera.main.ScreenToWorldPoint(pos);
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnDashPressed?.Invoke();
        }
    }
}