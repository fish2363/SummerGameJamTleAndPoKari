using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState.NotMoveEnemy
{
    public class EnemyIdleState : NotMoveEnemyState
    {
        public EnemyIdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }
        
        public override void Update()
        {
            base.Update();
            if (_attackCompo.CanAttack(_enemy.target))
            {
                _enemy.ChangeState("ATTACK");
            }
        }
    }
}