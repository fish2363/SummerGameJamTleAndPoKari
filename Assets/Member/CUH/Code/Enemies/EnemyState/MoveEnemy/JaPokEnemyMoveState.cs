using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class JaPokEnemyMoveState : MoveableEnemyState
    {
        private EntityMover _entityMover;
        
        public JaPokEnemyMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _entityMover = entity.GetCompo<EntityMover>();
        }

        public override void Update()
        {
            base.Update();
            Vector2 moveDir = (_enemy.target.transform.position - _enemy.transform.position).normalized;
            float dist = Vector2.Distance(_enemy.target.transform.position, _enemy.transform.position);
            _entityMover.SetMovement(moveDir);
            if (dist <= 1f)
            {
                _attackCompo.Attack();
            }
        }
    }
}