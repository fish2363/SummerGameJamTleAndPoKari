using Member.CUH.Code.Combat.Enemies;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class PlayerChaseFireAttack : EnemyAttackCompo
    {
        [SerializeField] private float _bulletSpeed = 1f;   // 탄환 속도
        
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
            Debug.Log("데미지");
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