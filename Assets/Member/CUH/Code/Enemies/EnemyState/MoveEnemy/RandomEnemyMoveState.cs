using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class RandomEnemyMoveState : MoveableEnemyState
    {
        private float halfXSize = 4.5f;
        private float halfYSize = 4.5f;
        private Vector2 nextPos;
        
        public RandomEnemyMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            
        }

        public override void Update()
        {
            base.Update();
            if (_attackCompo.CanAttack(_enemy.target))
            {
                _attackCompo.Attack();
                nextPos.x = Random.Range(-halfXSize, halfXSize);
                nextPos.y = Random.Range(-halfYSize, halfYSize);
                _enemy.transform.position = nextPos;
            }
        }
    }
}