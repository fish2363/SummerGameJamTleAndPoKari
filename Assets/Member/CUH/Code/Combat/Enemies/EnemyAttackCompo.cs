using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using Member.ISC.Code.Players;
using UnityEngine;
using UnityEngine.Serialization;

namespace Member.CUH.Code.Combat.Enemies
{
    public class EnemyAttackCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private float attackRange;
        [SerializeField] private float attackCooldown;
        
        protected Enemy _enemy;
        protected Player _target;
        private float _lastAtkTime;
        
        public virtual void Initialize(Entity entity)
        {
            _enemy = entity as Enemy;
        }
        
        public void AfterInitialize()
        {
            _target = _enemy.target;
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