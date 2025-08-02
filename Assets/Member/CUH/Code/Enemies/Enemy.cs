using System;
using System.Collections;
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
        [SerializeField] private SpriteRenderer[] sprites;
        
        private EntityStateMachine _stateMachine;

        private float _lifeTime;
        private bool _isOverClock = false;
        
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
            foreach (var sprite in sprites)
            {
                sprite.DOColor(Color.red, 30f).SetEase(Ease.InQuart);
            }
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
            _lifeTime += Time.deltaTime;
            if (_lifeTime >= 30f && !_isOverClock)
            {
                OnOverClock?.Invoke(true);
                _isOverClock = true;
            }
        }
        
        public void ChangeState(string newStateName, bool forced = false)
            => _stateMachine.ChangeState(newStateName,forced);

        public void ApplyDamage(float damage)
        {
            if(IsDead) return;
            IsDead = true;
            ComboManager.Instance.PlusCombo(transform);
            ApiManager.Instance.MinusGageValue(1);
            Instantiate(deadEffect, transform.position, Quaternion.identity);
            OnDeadEvent?.Invoke();
            if (_lifeTime >= 30f) OnOverClock?.Invoke(false);
            GetCompo<EntityAnimator>().GetComponent<SpriteRenderer>().DOKill();
            Destroy(gameObject);
        }

        private IEnumerator DeadRoutine()
        {
            GetCompo<EntityAnimator>().GetComponent<SpriteRenderer>().DOColor(Color.white,0.05f)
                .OnComplete(()=>transform.DOScale(new Vector2(0.1f,0.1f),0.2f));
            yield return new WaitForSeconds(0.4f);
        }

        public void KillSelf()
        {
            IsDead = true;
            OnDeadEvent?.Invoke();
            if (_lifeTime >= 30f) OnOverClock?.Invoke(false);
            GetCompo<EntityAnimator>().GetComponent<SpriteRenderer>().DOKill();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            transform.DOKill();
            foreach (var sprite in sprites)
            {
                sprite.DOKill();
            }
        }
    }
}