using Member.CUH.Code.Combat;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class PlayerHealth : MonoBehaviour, IEntityComponent, IDamageable
    {
        [SerializeField] private float currentHealth;
        [SerializeField] private float maxHealth;

        private Player _player;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
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