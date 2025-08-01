using System;
using System.Numerics;
using DG.Tweening;
using Member.CUH.Code.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

namespace Member.ISC.Code.Players
{
    public class PlayerMoveCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private Rigidbody2D rb; 
        
        [Header("이동속도")]
        [SerializeField] private float moveSpeed;

        [Header("대쉬 지속시간 쿨타임 세기")]
        [SerializeField] private float dashDuration;
        [field: SerializeField] public float DashCool { get; private set; }
        [SerializeField] private float dashPower;

        public UnityEvent OnDash;
        
        private Player _player;
        private PlayerHealth _playerHealth;
        private Tweener _tweener;
        
        private bool _canDash = true;
        private float _previousMoveSpeed;
        
        private Vector2 _moveDir;
        private Vector2 _velocity;
        private Vector2 _autoDir = Vector2.zero;

        
        public bool CanManualMovement { get; set; } = true;
        public float CurrentDashTime { get; set; }
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            
            _playerHealth = entity.GetCompo<PlayerHealth>();
        }
        
        public void AfterInitialize()
        {
            _player.PlayerInput.OnDashPressed += HandleDashPressed;

            _previousMoveSpeed = moveSpeed;
        }

        private void OnDestroy()
        {
            _player.PlayerInput.OnDashPressed -= HandleDashPressed;
            
            if (_tweener.IsActive())
                _tweener.Kill();
        }

        private void HandleDashPressed()
        {
            if (_canDash)
            {
                OnDash?.Invoke();
                Dash();
            }
        }

        public void SetDirection(Vector2 dir)
        {
            _moveDir = dir.normalized;
        }
        
        public void SetAutoMovement(Vector2 autoDir) => _autoDir = autoDir;

        private void Update()
        {
            CurrentDashTime += Time.deltaTime;

            if (CurrentDashTime >= DashCool)
            {
                _canDash = true;
                CurrentDashTime = 0;
            }
        }

        private void FixedUpdate()
        {
            CalculateMove();
            Move();
        }

        private void CalculateMove()
        {
            if (CanManualMovement)
                _velocity = _moveDir * moveSpeed;
            else
                _velocity = _autoDir * moveSpeed;
        }

        public void StopImmediately()
        {
            _moveDir = Vector2.zero;
        }

        private void Move()
        {
            rb.linearVelocity = _velocity;
        }

        private void Dash()
        {
            _canDash = false;
            _playerHealth.Ignore = true;
            
            CanManualMovement = false;
            if (_moveDir == Vector2.zero)
                _moveDir = _player.transform.right;
            
            SetAutoMovement(_moveDir);
            
            _tweener = DOVirtual.Float(moveSpeed, (moveSpeed*dashPower), dashDuration, (x) => moveSpeed = x)
                .OnComplete(() =>
                {
                    CanManualMovement = true;
                    moveSpeed = _previousMoveSpeed;
                    _playerHealth.Ignore = false;
                });
        }
    }
}