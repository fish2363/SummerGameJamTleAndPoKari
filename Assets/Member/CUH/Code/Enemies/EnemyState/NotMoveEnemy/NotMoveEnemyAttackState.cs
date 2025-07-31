using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;

namespace Member.CUH.Code.Enemies.EnemyState.NotMoveEnemy
{
    public class NotMoveEnemyAttackState : NotMoveEnemyState
    {
        
        public NotMoveEnemyAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            _attackCompo.Attack();
        }

        public override void Update()
        {
            base.Update();
            if(_isTriggerCall)
                _enemy.ChangeState("IDLE");
        }
    }
}