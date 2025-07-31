using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;

namespace Member.CUH.Code.Enemies.EnemyState.NotMoveEnemy
{
    public class NotMoveEnemyAttackState : NotMoveEnemyState
    {
        private EnemyAttackCompo _attackCompo;
        
        public NotMoveEnemyAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _attackCompo = entity.GetCompo<EnemyAttackCompo>();
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