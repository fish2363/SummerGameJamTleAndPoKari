using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Combat
{
    public class BossHealth : MonoBehaviour, IEntityComponent, IAfterInitialize, IDamageable
    {
        [field: SerializeField] public float currentHealth { get; private set; }
        [field: SerializeField] public float maxHealth { get; private set; }
        
        private Boss _boss;

        public void Initialize(Entity entity)
        {
            _boss = entity as Boss;
        }
        
        public void AfterInitialize()
        {
            currentHealth = maxHealth;
        }
                
        public void ApplyDamage(float damage)
        {
            if (_boss.IsDead) return;

            
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

            if (currentHealth <= 0)
                _boss.OnDeadEvent?.Invoke();
            
            _boss.OnHitEvent?.Invoke();
        }
    }
}