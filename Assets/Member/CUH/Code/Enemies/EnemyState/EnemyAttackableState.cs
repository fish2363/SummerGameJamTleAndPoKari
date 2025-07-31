using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;

namespace Member.CUH.Code.Enemies.EnemyState
{
    public abstract class EnemyAttackableState : EntityState
    {
        protected Enemy _enemy;
        protected EnemyAttackCompo _attackCompo;


        protected EnemyAttackableState(Entity entity, int animationHash, Enemy enemy, EnemyAttackCompo attackCompo) : base(entity, animationHash)
        {
            _enemy = enemy;
            _attackCompo = attackCompo;
        }
    }
}