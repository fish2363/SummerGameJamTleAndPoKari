using Member.CUH.Code.Entities;
using Member.CUH.Code.Entities.FSM;
using UnityEngine;

namespace Member.ISC.Code.Players.States
{
    public class PlayerState : EntityState
    {
        protected Player _player;
        protected PlayerMoveCompo _moveCompo;
        protected Vector2 _movementKey;

        
        protected readonly float _inputThreshold = 0.1f;

        public PlayerState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _player = entity as Player;
            _moveCompo = entity.GetCompo<PlayerMoveCompo>();
            Debug.Assert(_player != null, "Player State is only For Player");
        }

        public override void Update()
        {
            base.Update();
            _movementKey = _player.PlayerInput.MovementKey;
            _moveCompo.SetDirection(_movementKey);
            Vector2 dir = (Vector2)_player.transform.position - _player.PlayerInput.MousePos;
        }
    }
}