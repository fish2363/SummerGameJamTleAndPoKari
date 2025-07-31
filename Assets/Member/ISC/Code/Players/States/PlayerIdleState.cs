using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players.States
{
    public class PlayerIdleState : PlayerState
    {
        private PlayerMoveCompo _moveCompo;


        public PlayerIdleState(Entity entity, int animationHash, PlayerMoveCompo moveCompo) : base(entity, animationHash)
        {
            _moveCompo = entity.GetCompo<PlayerMoveCompo>();
        }

        public override void Update()
        {
            base.Update();
            Vector2 movementKey = _player.PlayerInput.MovementKey;
            _moveCompo.SetDirection(movementKey);
            if (movementKey.magnitude > _inputThreshold)
            {
                const string move = "MOVE";
                _player.ChangeState(move);
            }
        }
    }
}