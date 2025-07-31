using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players.States
{
    public class PlayerIdleState : PlayerCanAttackState
    {
        public PlayerIdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Update()
        {
            base.Update();
            if (_movementKey.magnitude > _inputThreshold)
            {
                const string move = "MOVE";
                _player.ChangeState(move);
            }
        }
    }
}