using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState
{
    public abstract class EnemyCanAttackState : EntityState
    {
        protected Enemy _enemy;
        protected EnemyAttackCompo _attackCompo;
        
        protected EnemyCanAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _enemy = entity as Enemy;
            _attackCompo = entity.GetCompo<EnemyAttackCompo>(true);
        }
    }
}