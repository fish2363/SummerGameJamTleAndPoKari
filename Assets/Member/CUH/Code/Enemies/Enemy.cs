using System;
using Blade.FSM;
using Chuh007Lib.Dependencies;
using Member.CUH.Code.Combat;
using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;
using Member.ISC.Code.Players;
using UnityEngine;

namespace Member.CUH.Code.Enemies
{
    public class Enemy : Entity, IDamageable
    {
        [HideInInspector] public Player target;
        
        [SerializeField] private StateDataSO[] states;

        [SerializeField] private bool isMoveableEnemy;
        
        private EntityStateMachine _stateMachine;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, states);
            OnDeadEvent.AddListener(HandleDeadEvent);
        }
        
        private void HandleDeadEvent()
        {
            if(IsDead) return;
            IsDead = true;
            ChangeState("DEAD", true);
        }
        
        protected override void Start()
        {
            _stateMachine.ChangeState(isMoveableEnemy ? "MOVE" : "IDLE");
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }
        
        public void ChangeState(string newStateName, bool forced = false)
            => _stateMachine.ChangeState(newStateName,forced);

        public void ApplyDamage(float damage)
        {
            if(IsDead) return;
            IsDead = true;
            OnDeadEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}