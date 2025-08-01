using Member.CUH.Code.Entities;
using Member.KDH.Code.Bullet.AttackType;
using System.Collections;
using UnityEngine;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public class RandomEnemyMoveState : MoveableEnemyState
    {
        
        private RandomAttack randomAttack;
        
        public RandomEnemyMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            randomAttack = entity.GetCompo<RandomAttack>();
        }

        public override void Update()
        {
            base.Update();
            if (_attackCompo.CanAttack())
            {
                _attackCompo.Attack();
            }
        }

        
    }
}