using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players.States
{
    public class PlayerAttackChargeState : PlayerState
    {
        
        public PlayerAttackChargeState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _player.SetActiveFrame(true);
            _player.PlayerInput.OnAttackCanceled += HandleAttackCanceled;
        }

        public override void Exit()
        {
            _player.SetActiveFrame(false);
            _player.PlayerInput.OnAttackCanceled -= HandleAttackCanceled;
            base.Exit();
        }

        private void HandleAttackCanceled()
        {
            _player.ChangeState("ATTACK");
        }
    }
}