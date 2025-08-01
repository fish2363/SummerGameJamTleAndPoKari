using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;

namespace Member.CUH.Code.Enemies.BossStates
{
    public class BossAttackState : BossState
    {
        private BossAttackCompo _attackCompo;
        
        public BossAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _attackCompo = entity.GetCompo<BossAttackCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _animatorTrigger.OnAttackTrigger += HandleAttackTrigger;
            _animatorTrigger.OnAnimationEndTrigger += HandleAnimEnd;
        }
        private void HandleAttackTrigger()
        {
            _attackCompo.Attack();
        }
        
        private void HandleAnimEnd()
        {
            _boss.ChangeState("IDLE");
        }
        
        public override void Exit()
        {
            _animatorTrigger.OnAttackTrigger -= HandleAttackTrigger;
            _animatorTrigger.OnAnimationEndTrigger -= HandleAnimEnd;
            base.Exit();
        }
    }
}