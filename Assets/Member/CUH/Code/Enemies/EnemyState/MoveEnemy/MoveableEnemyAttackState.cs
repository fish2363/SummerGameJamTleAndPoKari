using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class MoveableEnemyAttackState : MoveableEnemyState
    {
        private EnemyAttackCompo _attackCompo;
        
        public MoveableEnemyAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _attackCompo = entity.GetCompo<EnemyAttackCompo>(true);
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
                _enemy.ChangeState("MOVE");
        }
    }
}