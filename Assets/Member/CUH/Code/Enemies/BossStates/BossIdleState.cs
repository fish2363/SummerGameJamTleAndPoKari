using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

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
                string animString = _attackCompo.GetRandomAttack();
                Debug.Log(animString);
                _boss.ChangeState(animString);
            }
        }
    }
}