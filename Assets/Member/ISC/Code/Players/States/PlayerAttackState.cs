using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players.States
{
    public class PlayerAttackState : PlayerState
    {
        private PlayerAttackCompo _attackCompo;
        
        public PlayerAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _attackCompo = entity.GetCompo<PlayerAttackCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _attackCompo.Attack();
        }
        
        public override void Update()
        {
            base.Update();

            if (_isTriggerCall)
                _player.ChangeState("IDLE");
        }
    }
}