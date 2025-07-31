using DG.Tweening;
using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class BasicEnemyAttack : EnemyAttackCompo
    {
        [SerializeField] private float bombDelay;
        
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
                    DOVirtual.DelayedCall(bombDelay, () =>
                    {
                        _target.ApplyDamage(1);
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
    }
}