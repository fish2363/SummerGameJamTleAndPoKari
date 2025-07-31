using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class CircularAttack : EnemyAttackCompo
    {
        [Header("원형 공격 설정")]
        [SerializeField] private int _bulletCount = 20; // 발사할 탄환 개수
        [SerializeField] private float _attackInterval = 2f; // 공격 간격 (초)
        [SerializeField] private float _bulletSpeed = 1f; // 탄환 속도
        
        private float _lastAttackTime;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            Debug.Log($"{gameObject.name}: 원형 공격 컴포넌트가 초기화되었습니다. (탄환 수: {_bulletCount}개)");
        }
        
        private void Update()
        {
            if (Time.time - _lastAttackTime >= _attackInterval)
            {
                Attack();
            }
        }
        
        public override void Attack()
        {
            base.Attack();
            
            if (BulletPool.Instance == null)
            {
                Debug.LogError($"{gameObject.name}: BulletPool이 준비되지 않았습니다!");
                return;
            }
            
            float angleStep = 360f / _bulletCount;
            
            for (int i = 0; i < _bulletCount; i++)
            {
                float currentAngle = i * angleStep;

                float radians = currentAngle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                );
                
                Bullet bullet = BulletPool.Instance.GetBullet();
                if (bullet == null)
                {
                    Debug.LogWarning($"{gameObject.name}: {i + 1}번째 탄환을 생성할 수 없습니다!");
                    continue;
                }
                
                bullet.transform.position = transform.position;
                
                bullet.Fire(direction, _bulletSpeed);
            }
            
            _lastAttackTime = Time.time;
            
            Debug.Log($"{gameObject.name}: 원형 공격 실행 완료! {_bulletCount}발 발사됨");
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
            
            if (_bulletCount > 0)
            {
                float angleStep = 360f / _bulletCount;
                Gizmos.color = Color.yellow;
                
                for (int i = 0; i < _bulletCount; i++)
                {
                    float currentAngle = i * angleStep;
                    float radians = currentAngle * Mathf.Deg2Rad;
                    Vector2 direction = new Vector2(
                        Mathf.Cos(radians),
                        Mathf.Sin(radians)
                    );
                    
                    Vector3 endPoint = transform.position + (Vector3)(direction * 2f);
                    Gizmos.DrawLine(transform.position, endPoint);
                }
            }
        }
    }
}