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
        }
        
        public void SetTarget(IDamageable target)
        {
            Target = target;
            AfterInitialize();
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