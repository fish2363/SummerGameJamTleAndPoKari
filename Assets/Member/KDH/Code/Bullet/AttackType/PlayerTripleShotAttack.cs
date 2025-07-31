using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class PlayerTripleShotAttack : EnemyAttackCompo
    {
        [Header("3점사 공격 설정")]
        [SerializeField] private float _attackInterval = 1f; // 공격 간격 (초)
        [SerializeField] private float _bulletSpeed = 1f; // 탄환 속도
        [SerializeField] private float _spreadAngle = 15f; // 탄환 퍼짐 각도 (도)
        
        private Transform _playerTransform;
        private float _lastAttackTime;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            FindPlayer();
        }
        
        private void Update()
        {
            if (_playerTransform == null)
            {
                FindPlayer();
                return;
            }
            
            if (Time.time - _lastAttackTime >= _attackInterval)
            {
                Attack();
            }
        }
        
        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("플레이어를 찾을 수 없습니다!");
            }
        }
        
        public override void Attack()
        {
            base.Attack();
            
            if (_playerTransform == null || BulletPool.Instance == null) return;
            
            Vector2 baseDirection = (_playerTransform.position - transform.position).normalized;
            
            float[] angles = { 0f, -_spreadAngle, _spreadAngle };
            
            foreach (float angle in angles)
            {
                Vector2 direction = RotateVector(baseDirection, angle);
                
                Bullet bullet = BulletPool.Instance.GetBullet();
                bullet.transform.position = transform.position;
                bullet.Fire(direction, _bulletSpeed);
            }
            
            _lastAttackTime = Time.time;
        }
        
        private Vector2 RotateVector(Vector2 vector, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            
            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }
    }
}