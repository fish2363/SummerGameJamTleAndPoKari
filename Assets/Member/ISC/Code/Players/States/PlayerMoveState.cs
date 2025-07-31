using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players.States
{
    public class PlayerMoveState : PlayerCanAttackState
    {
        public PlayerMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Update()
        {
            base.Update();

            Vector2 movementKey = _player.PlayerInput.MovementKey;
            _moveCompo.SetDirection(movementKey);
            if (movementKey.magnitude < _inputThreshold)
            {
                const string idle = "IDLE";
                _player.ChangeState(idle);
            }
        }
    }
}