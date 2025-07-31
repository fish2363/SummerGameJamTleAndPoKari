using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies
{
    public abstract class EnemyAttackCompo : MonoBehaviour, IEntityComponent
    {
        protected Enemy _enemy;
        private float _lastAtkTime;
        
        public virtual void Initialize(Entity entity)
        {
            _enemy = entity as Enemy;
        }

        public virtual void Attack()
        {
            _lastAtkTime = Time.time;
        }
    }
}