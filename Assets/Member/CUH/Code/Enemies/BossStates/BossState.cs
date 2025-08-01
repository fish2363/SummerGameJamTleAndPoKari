using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;

namespace Member.CUH.Code.Enemies.BossStates
{
    public abstract class BossState : EntityState
    {
        protected Boss _boss;
        
        public BossState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _boss = entity as Boss;
        }
    }
}