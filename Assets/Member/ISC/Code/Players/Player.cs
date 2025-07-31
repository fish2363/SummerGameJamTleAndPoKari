using System;
using Blade.FSM;
using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;
using Member.ISC.Code.Managers;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class Player : Entity
    {
        [field: SerializeField] public InputManagerSO PlayerInput { get; private set; }
        
        [SerializeField] private StateDataSO[] stateDataList;
        
        private EntityStateMachine _stateMachine;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, stateDataList);
            
            OnHitEvent.AddListener(HandleHitEvent);
            OnDeadEvent.AddListener(HandleDeadEvent);
        }

        private void OnDestroy()
        {
            OnHitEvent.RemoveListener(HandleHitEvent);
            OnDeadEvent.RemoveListener(HandleDeadEvent);
        }

        private void HandleDeadEvent()
        {
            
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
        
        public void ChangeState(string newStateName, bool force = false) 
            => _stateMachine.ChangeState(newStateName, force);
    }
}