using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class RotatingAttack : EnemyAttackCompo
    {
        [Header("회전 공격 설정")]
        // [SerializeField] private float _attackInterval = 0.3f; // 공격 간격 (초)
        [SerializeField] private float _bulletSpeed = 1f; // 탄환 속도
        [SerializeField] private float _rotationStep = 3f; // 회전 각도 (도)
        [SerializeField] private float _initialAngle = 0f; // 시작 각도 (도)
        
        private float _lastAttackTime;
        private float _currentAngle; // 현재 발사 각도
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _currentAngle = _initialAngle;
            
            // Debug.Log($"{gameObject.name}: 회전 공격 컴포넌트가 초기화되었습니다. (회전 간격: {_rotationStep}도, 공격 간격: {_attackInterval}초)");
        }
        
        // private void Update()
        // {
        //     if (Time.time - _lastAttackTime >= _attackInterval)
        //     {
        //         Attack();
        //     }
        // }
        
        public override void Attack()
        {
            base.Attack();

            if (BulletPool.Instance == null)
            {
                Debug.LogError($"{gameObject.name}: BulletPool이 준비되지 않았습니다!");
                return;
            }
            
            float radians = _currentAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );
            
            Bullet bullet = BulletPool.Instance.GetBullet();
            if (bullet == null)
            {
                Debug.LogWarning($"{gameObject.name}: 탄환을 생성할 수 없습니다!");
                return;
            }
            
            bullet.transform.position = transform.position;
            
            bullet.Fire(direction, _bulletSpeed);
            
            _enemy.transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle - 90f);
            
            _currentAngle += _rotationStep;
            
            if (_currentAngle >= 360f)
            {
                _currentAngle -= 360f;
            }
            
            _lastAttackTime = Time.time;
            
            Debug.Log($"{gameObject.name}: 회전 공격 실행! 각도: {_currentAngle - _rotationStep:F1}도");
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            float radians = _currentAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );
            Vector3 endPoint = transform.position + (Vector3)(direction * 3f);
            Gizmos.DrawLine(transform.position, endPoint);
            
            Gizmos.color = Color.gray;
            for (int i = 1; i <= 5; i++)
            {
                float futureAngle = _currentAngle + (_rotationStep * i);
                float futureRadians = futureAngle * Mathf.Deg2Rad;
                Vector2 futureDirection = new Vector2(
                    Mathf.Cos(futureRadians),
                    Mathf.Sin(futureRadians)
                );
                Vector3 futureEndPoint = transform.position + (Vector3)(futureDirection * 2f);
                Gizmos.DrawLine(transform.position, futureEndPoint);
            }
        }
    }
}