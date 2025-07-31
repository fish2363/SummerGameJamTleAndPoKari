using System;
using Member.CUH.Code.Combat;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class PlayerHealth : MonoBehaviour, IEntityComponent, IDamageable, IAfterInitialize
    {
        [field: SerializeField] public float currentHealth { get; private set; }
        [field: SerializeField] public float maxHealth { get; private set; }

        private Player _player;

        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        public void AfterInitialize()
        {
            currentHealth = maxHealth;
        }
        
        public void ApplyDamage(float damage)
        {
            if (_player.IsDead) return;
            
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

            if (currentHealth <= 0)
                _player.OnDeadEvent?.Invoke();
            
            _player.OnHitEvent?.Invoke();
        }

    }
}