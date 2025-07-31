using System;
using UnityEngine;

namespace Member.CUH.Code.Entities
{
    public class EntityMover : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private float _moveSpeed = 5f;
    
        public bool CanManualMove { get; set; } = true;
        public Vector2 Velocity => _rbCompo.linearVelocity;
        public event Action<Vector2> OnMoveVelocity;

        private Entity _entity;
        private EntityRenderer _renderer;
        private Rigidbody2D _rbCompo;
        private Vector2 _movement;
    
        private float _moveSpeedMultiplier;
    
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _renderer = _entity.GetCompo<EntityRenderer>();
            _rbCompo = entity.GetComponent<Rigidbody2D>();
        
            _moveSpeedMultiplier = 1f;
        }
    
        public void SetMoveSpeedMultiplier(float value) => _moveSpeedMultiplier = value;

        public void AddForceToEntity(Vector2 force)
        {
            _rbCompo.AddForce(force, ForceMode2D.Impulse);
        }

        public void StopImmediately()
        {
            _rbCompo.linearVelocity = Vector2.zero;
            _movement = Vector2.zero;
        }
    
        public void SetMovement(Vector2 value)
        {
            _movement = value;
        }
    
        private void FixedUpdate()
        {
            MoveCharacter();
        }
        
        private void MoveCharacter()
        {
            if (CanManualMove)
            {
                _renderer.FlipController(_movement.x);
                _rbCompo.linearVelocity = _movement * (_moveSpeed * _moveSpeedMultiplier);
            }
        
            OnMoveVelocity?.Invoke(_rbCompo.linearVelocity);
        }
    }
}
