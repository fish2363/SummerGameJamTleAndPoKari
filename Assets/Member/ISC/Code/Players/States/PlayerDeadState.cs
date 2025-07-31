using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players.States
{
    public class PlayerDeadState : PlayerState
    {
        public PlayerDeadState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _moveCompo.CanManualMovement = false;
            _player.SetActiveFrame(false);
        }

        public override void Update()
        {
            base.Update();
            Debug.Log("죽음!");
        }
    }
}