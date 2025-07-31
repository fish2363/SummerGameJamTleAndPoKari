using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class RandomEnemyMoveState : MoveableEnemyState
    {
        private float _halfXSize = 4.5f;
        private float _halfYSize = 4.5f;
        private Vector2 _nextPos;
        
        public RandomEnemyMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            
        }

        public override void Update()
        {
            base.Update();
            if (_attackCompo.CanAttack())
            {
                _attackCompo.Attack();
                _nextPos.x = Random.Range(-_halfXSize, _halfXSize);
                _nextPos.y = Random.Range(-_halfYSize, _halfYSize);
                _enemy.transform.position = _nextPos;
            }
        }
    }
}