using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class DistanceEnemyMoveState : MoveableEnemyState
    {
        private EntityMover _entityMover;
        
        public DistanceEnemyMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _entityMover = entity.GetCompo<EntityMover>();
        }
        
        public override void Update()
        {
            base.Update();
            Vector2 moveDir = (_enemy.target.transform.position - _enemy.transform.position).normalized;
            float dist = Vector2.Distance(_enemy.target.transform.position, _enemy.transform.position);
            if (dist >= 5f)
                _entityMover.SetMovement(moveDir);
            else if (dist <= 3f)
                _entityMover.SetMovement(-moveDir);
            else 
                _entityMover.SetMovement(Vector2.zero);
            if (_attackCompo.CanAttack(_enemy.target))
            {
                _attackCompo.Attack();
            }
        }
    }
}