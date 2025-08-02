using System;
using Ami.BroAudio;
using DG.Tweening;
using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class BasicEnemyAttack : EnemyAttackCompo
    {
        [SerializeField] private float bombDelay;
        [SerializeField] private float bombRadius = 2f;
        [SerializeField] private LayerMask whatIsTarget;
        [SerializeField] private ParticleSystem bombParticle;
        [SerializeField] private SoundID explosionSound;
        
        private Tween _tween;
        
        public override void Initialize(Entity entity)
        {
            try
            {
                base.Initialize(entity);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] BasicEnemyAttack 초기화 중 오류 발생: {ex.Message}");
            }
        }
        
        public override void Attack()
        {
            try
            {
                base.Attack();
                
                if (_target != null)
                {
                    _enemy.GetCompo<EntityMover>().StopImmediately();
                    _tween = DOVirtual.DelayedCall(bombDelay, () =>
                    {
                    Collider2D targetCol = Physics2D.OverlapCircle(transform.position, bombRadius, whatIsTarget);
                    if (targetCol != null)
                    {
                        _target.ApplyDamage(1);
                    }
                    explosionSound.Play();
                    Instantiate(bombParticle,_enemy.transform.position,Quaternion.identity);
                    }).OnComplete(() => _enemy.KillSelf());
                }
                else
                {
                    Debug.LogWarning($"[{gameObject.name}] 타겟이 설정되지 않았습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Attack 메서드 실행 중 오류 발생: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            _tween.Kill();
        }
    }
}