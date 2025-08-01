using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;

namespace Member.CUH.Code.Enemies.BossStates
{
    public class BossIdleState : BossState
    {
        private BossAttackCompo _attackCompo;

        public BossIdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _attackCompo = entity.GetCompo<BossAttackCompo>();
        }

        public override void Update()
        {
            base.Update();
            if (_attackCompo.CanAttack())
            {
                _boss.ChangeState("ATTACK");
            }
        }
    }
}