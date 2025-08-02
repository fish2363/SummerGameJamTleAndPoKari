using System;
using System.Collections;
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

        public bool IsHitting { get; set; } = false;

        public bool CanAttack { get; set; } = false;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, stateDataList);
            
            OnHitEvent.AddListener(HandleHitEvent);
            OnDeadEvent.AddListener(HandleDeadEvent);

            SetActiveFrame(false);

            CanAttack = false;
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

            StartCoroutine(PlayNextAnimation());
        }

        private IEnumerator PlayNextAnimation()
        {
            yield return null;
            yield return null;
            
            const string idle = "IDLE";
            ChangeState(idle);

            yield return null;

            CanAttack = true;
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }

        private void FixedUpdate()
        {
            if (!IsDead)
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