using System;
using Blade.FSM;
using Chuh007Lib.Dependencies;
using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;
using Member.ISC.Code.Managers;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class Player : Entity, IDependencyProvider
    {
        [field: SerializeField] public InputManagerSO PlayerInput { get; private set; }
        
        [SerializeField] private StateDataSO[] stateDataList;
        
        private EntityStateMachine _stateMachine;

        [Provide]
        private Player ProviderPlayer() => this;
        
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
        
        public void ChangeState(string newStateName, bool force = false) 
            => _stateMachine.ChangeState(newStateName, force);
    }
}