using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;

namespace Member.CUH.Code.Enemies.EnemyState.NotMoveEnemy
{
    public abstract class NotMoveEnemyState : EnemyCanAttackState
    {
        public NotMoveEnemyState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            
        }
    }
}