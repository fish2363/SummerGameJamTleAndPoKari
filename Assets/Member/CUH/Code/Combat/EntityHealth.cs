using System;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Combat
{
    public class EntityHealth : MonoBehaviour, IEntityComponent, IDamageable
    {
        [SerializeField] private float _maxHealth = 50f;
        [SerializeField] private float _currentHealth;
        
        private Entity _entity;

        public event Action<Entity> OnHit;
        public event Action OnDeath;

        public bool IsHit = false;
        public bool IsDead = false;
    
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _currentHealth = _maxHealth;
        }


        public void ApplyDamage(float damage, Entity dealer)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
            OnHit?.Invoke(dealer);
            IsHit = true;
            if (_currentHealth <= 0)
            {
                IsDead = true;
                OnDeath?.Invoke();
            }
        }

        public void ApplyDamage(float damage)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
            IsHit = true;
            if (_currentHealth <= 0)
            {
                IsDead = true;
                OnDeath?.Invoke();
            }
        }

    }
}
