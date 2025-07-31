using System;
using Blade.FSM;
using Chuh007Lib.Dependencies;
using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;
using Member.ISC.Code.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.ISC.Code.Players
{
    public class Player : Entity, IDependencyProvider
    {
        [field: SerializeField] public InputManagerSO PlayerInput { get; private set; }
        
        [SerializeField] private StateDataSO[] stateDataList;
        [SerializeField] private GameObject attackFrame;
        
        private EntityStateMachine _stateMachine;

        [Provide]
        private Player ProviderPlayer() => this;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, stateDataList);
            
            OnHitEvent.AddListener(HandleHitEvent);
            OnDeadEvent.AddListener(HandleDeadEvent);

            SetActiveFrame(false);
        }

        private void OnDestroy()
        {
            OnHitEvent.RemoveListener(HandleHitEvent);
            OnDeadEvent.RemoveListener(HandleDeadEvent);
        }

        private void HandleDeadEvent()
        {
            if (IsDead) return;
            IsDead = true;
            ChangeState("DEAD", true);
        }

        private void HandleHitEvent()
        {
            
        }

        protected override void Start()
        {
            base.Start();
            
            const string idle = "IDLE";
            _stateMachine.ChangeState(idle);
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }

        private void FixedUpdate()
        {
            Rotate(PlayerInput.MousePos);
        }

        private void Rotate(Vector2 pos)
        {
            Vector2 dir = pos - (Vector2)transform.position; 
            float q = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            transform.rotation = Quaternion.Euler(0, 0, q);
        }

        public void SetActiveFrame(bool isActive)
        {
            attackFrame.SetActive(isActive);
        }
        
        public void ChangeState(string newStateName, bool force = false) 
            => _stateMachine.ChangeState(newStateName, force);
    }
}