using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class MoveableEnemyAttackState : MoveableEnemyState
    {
        private EntityMover _mover;
        
        public MoveableEnemyAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately();
            _canRotate = false;
            _attackCompo.Attack();
        }

        public override void Update()
        {
            base.Update();
            if (_isTriggerCall)
            {
                _canRotate = false;
                _enemy.ChangeState("MOVE");
            }
        }
    }
}