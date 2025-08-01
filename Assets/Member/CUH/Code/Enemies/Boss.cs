using System;
using Blade.FSM;
using Member.CUH.Code.Combat;
using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;
using UnityEngine;

namespace Member.CUH.Code.Enemies
{
    public class Boss : Entity
    {
        public IDamageable Target;
        
        [SerializeField] private StateDataSO[] states;
        
        private EntityStateMachine _stateMachine;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, states);
            OnDeadEvent.AddListener(HandleDeadEvent);
            OnHitEvent.AddListener(HandleHitEvent);
        }
        
        public void SetTarget(IDamageable target)
        {
            Target = target;
            AfterInitialize();
        }
        
        private void HandleHitEvent()
        {
            ComboManager.Instance.PlusCombo(transform);
        }
        
        private void HandleDeadEvent()
        {
            if(IsDead) return;
            IsDead = true;
            Destroy(gameObject);
        }
        
        protected override void Start()
        {
            base.Start();
            _stateMachine.ChangeState("IDLE");
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }
        
        public void ChangeState(string newStateName, bool forced = false)
            => _stateMachine.ChangeState(newStateName,forced);
        
    }
}