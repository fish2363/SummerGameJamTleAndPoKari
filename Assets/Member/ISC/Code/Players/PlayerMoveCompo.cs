using System;
using System.Numerics;
using Member.CUH.Code.Entities;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Member.ISC.Code.Players
{
    public class PlayerMoveCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Rigidbody2D rb; 
        
        [Header("이동속도")]
        [SerializeField] private float moveSpeed;
        
        private Player _player;

        private Vector2 _moveDir;
        private Vector2 _velocity;

        public bool CanManualMovement { get; set; } = true;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        public void SetDirection(Vector2 dir)
        {
            _moveDir = dir.normalized;
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
                _velocity = Vector2.zero;
        }

        public void StopImmediately()
        {
            _moveDir = Vector2.zero;
        }

        private void Move()
        {
            rb.linearVelocity = _velocity;
        }
    }
}