using DG.Tweening;
using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class PlayerChaseFireAttack : EnemyAttackCompo
    {
        [SerializeField] private float _bulletSpeed = 1f;   // 탄환 속도
        [SerializeField] private float bombDelay;
        
        private void Update()
        {
            if (CanAttack())
            {
                FireBullet();
            }
        }
        
        public override void Attack()
        {
            base.Attack();
            _enemy.GetCompo<EntityMover>().StopImmediately();
            DOVirtual.DelayedCall(bombDelay, () =>
            {
                _target.ApplyDamage(1);
            }).OnComplete(() => _enemy.KillSelf());
        }

        private void FireBullet()
        {
            if (_target == null || BulletPool.Instance == null) return;
            
            Vector2 direction = (_target.transform.position - transform.position).normalized;
            
            Bullet bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = transform.position;
            bullet.Fire(direction, _bulletSpeed);
            _lastAtkTime = Time.time;
        }
    }
}