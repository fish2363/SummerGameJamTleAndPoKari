using System;
using Blade.FSM;
using Chuh007Lib.Dependencies;
using DG.Tweening;
using Member.CUH.Code.Combat;
using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;
using Member.ISC.Code.Players;
using UnityEngine;

namespace Member.CUH.Code.Enemies
{
    public class Enemy : Entity, IDamageable
    {
        public event Action<bool> OnOverClock;
        
        [HideInInspector] public IDamageable Target;
        
        [SerializeField] private StateDataSO[] states;

        [SerializeField] private bool isMoveableEnemy;

        [SerializeField] private ParticleSystem deadEffect;

        private EntityStateMachine _stateMachine;

        private float _lifeTime;
        
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

        public void SetTarget(IDamageable target)
        {
            Target = target;
            AfterInitialize();
        }
        
        protected override void Start()
        {
            _stateMachine.ChangeState(isMoveableEnemy ? "MOVE" : "IDLE");
            GetComponentInChildren<SpriteRenderer>().DOColor(Color.red, 30f).SetEase(Ease.InQuart);
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
            if (_lifeTime >= 30f)
            {
                OnOverClock?.Invoke(true);
            }
        }
        
        public void ChangeState(string newStateName, bool forced = false)
            => _stateMachine.ChangeState(newStateName,forced);

        public void ApplyDamage(float damage)
        {
            if(IsDead) return;
            IsDead = true;
            Instantiate(deadEffect,transform.position,Quaternion.identity);
            OnDeadEvent?.Invoke();
            if (_lifeTime >= 30f) OnOverClock?.Invoke(false);

            Destroy(gameObject);
        }

        public void KillSelf()
        {
            IsDead = true;
            OnDeadEvent?.Invoke();
            if (_lifeTime >= 30f) OnOverClock?.Invoke(false);
            Destroy(gameObject);
        }
    }
}