using Ami.BroAudio;
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

            _animatorTrigger.OnPlaySoundTrigger += PlaySound;
            _attackCompo.Attack();
        }
        
        public override void Update()
        {
            base.Update();

            if (_isTriggerCall)
                _player.ChangeState("IDLE");
        }

        public override void Exit()
        {
            _animatorTrigger.OnPlaySoundTrigger -= PlaySound;
            base.Exit();
        }

        private void PlaySound()
        {
            int idx = Random.Range(0, _attackCompo.attackSwingSounds.Length);
            _attackCompo.attackSwingSounds[idx].Play();
        }
    }
}