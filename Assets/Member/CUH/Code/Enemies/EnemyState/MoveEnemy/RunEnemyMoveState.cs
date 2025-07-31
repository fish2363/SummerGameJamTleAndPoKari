using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class RunEnemyMoveState : MoveableEnemyState
    {
        private EntityMover _entityMover;
        
        public RunEnemyMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _entityMover = entity.GetCompo<EntityMover>();
        }

        public override void Update()
        {
            base.Update();
            Vector2 moveDir = (_enemy.target.transform.position - _enemy.transform.position).normalized;
            
            _entityMover.SetMovement(-moveDir);
            if (_attackCompo.CanAttack(_enemy.target))
            {
                _enemy.ChangeState("ATTACK");
            }
        }
    }
}