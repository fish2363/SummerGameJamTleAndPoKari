using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;

namespace Member.CUH.Code.Enemies.EnemyState.MoveEnemy
{
    public abstract class MoveableEnemyState : EnemyCanAttackState
    {
        protected bool _canRotate = true;
        
        private EntityRenderer _renderer;
        
        public MoveableEnemyState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _renderer = entity.GetCompo<EntityRenderer>();
        }

        public override void Update()
        {
            base.Update();
            if(_canRotate)
                _renderer.RotateToTarget(_enemy.Target.transform);
        }
    }
}