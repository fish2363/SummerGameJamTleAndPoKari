using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class JaPokEnemyMoveState : MoveableEnemyState
    {
        private EntityMover _entityMover;
        private bool isBomb = false;
        
        public JaPokEnemyMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _entityMover = entity.GetCompo<EntityMover>();
        }

        public override void Update()
        {
            base.Update();
            if(isBomb) return;
            
            Vector2 moveDir = (_enemy.Target.transform.position - _enemy.transform.position).normalized;
            float dist = Vector2.Distance(_enemy.Target.transform.position, _enemy.transform.position);
            _entityMover.SetMovement(moveDir);
            if (dist <= 1.5f)
            {
                isBomb = true;
                _attackCompo.Attack();
            }
        }
    }
}