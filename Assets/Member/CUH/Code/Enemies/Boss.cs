using System;
using System.Collections;
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
        [SerializeField] private ParticleSystem deathEffect;
        private EntityStateMachine _stateMachine;
        
        [SerializeField] private SpriteRenderer[] sprites;
        [SerializeField] private Color blinkColor;
        [SerializeField] private Color nomalColor;
        [SerializeField] private float blinkTime;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, states);
            OnDeadEvent.AddListener(HandleDeadEvent);
            OnHitEvent.AddListener(HandleHitEvent);
            sprites = GetComponentsInChildren<SpriteRenderer>();
        }
        
        public void SetTarget(IDamageable target)
        {
            Target = target;
            AfterInitialize();
        }
        
        private void HandleHitEvent()
        {
            if(IsDead) return;
            ComboManager.Instance.PlusCombo(transform);
            StartCoroutine(BlinkFeedback());
        }

        private IEnumerator BlinkFeedback()
        {
            foreach (var sprite in sprites)
            {
                sprite.color = blinkColor;
            }
            yield return new WaitForSeconds(blinkTime);
            foreach (var sprite in sprites)
            {
                sprite.color = nomalColor;
            }
        }

        private void HandleDeadEvent()
        {
            if(IsDead) return;
            IsDead = true;
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            StartCoroutine(DeadRoutine());
        }

        private IEnumerator DeadRoutine()
        {
            Time.timeScale = 0f;
            ApiManager.Instance.ShakeScreen();
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 1f;
            StageChangeManager.Instance.ClosePanels();
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