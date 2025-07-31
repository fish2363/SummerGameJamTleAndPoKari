using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Member.CUH.Code.Combat.Enemies
{
    public class EnemyAttackCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private float attackRange;
        [SerializeField] private float attackCooldown;
        
        protected Enemy _enemy;
        private float _lastAtkTime;
        
        public virtual void Initialize(Entity entity)
        {
            _enemy = entity as Enemy;
        }

        public virtual bool CanAttack(Transform target)
        {
            return _lastAtkTime + attackCooldown < Time.time && 
                   Vector2.Distance(target.transform.position, transform.position) <= attackRange;
        }
        
        public virtual void Attack()
        {
            Debug.Log("저놈추");
            _lastAtkTime = Time.time;
        }
    }
}