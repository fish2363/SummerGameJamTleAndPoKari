using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using Member.ISC.Code.Players;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies
{
    public class EnemyAttackCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private float attackRange = 100f;
        [SerializeField] private float attackCooldown = 1f;
        
        protected Enemy _enemy;
        protected IDamageable _target;
        protected float _lastAtkTime;
        
        public virtual void Initialize(Entity entity)
        {
            _enemy = entity as Enemy;
        }
        
        public void AfterInitialize()
        {
            _target = _enemy.Target;
        }
        
        public virtual bool CanAttack()
        {
            return _lastAtkTime + attackCooldown < Time.time && 
                   Vector2.Distance(_target.transform.position, transform.position) <= attackRange;
        }
        
        public virtual void Attack()
        {
            Debug.Log("저놈추");
            _lastAtkTime = Time.time;
        }

    }
}