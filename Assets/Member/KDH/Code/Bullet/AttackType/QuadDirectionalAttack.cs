using Ami.BroAudio;
using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class QuadDirectionalAttack : EnemyAttackCompo
    {
        [Header("4방향 공격 설정")]
        // [SerializeField] private float _attackInterval = 0.5f; // 공격 간격 (초)
        [SerializeField] private float _bulletSpeed = 1f; // 탄환 속도
        [SerializeField] private float _rotationStep = 45f; // 회전 각도 (도) - 4방향이므로 45도
        [SerializeField] private float _initialAngle = 0f; // 시작 각도 (도)
        [SerializeField] private SoundID[] enemyAttackSounds;
        
        private float _lastAttackTime;
        private float _currentAngle; // 현재 발사 각도
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _currentAngle = _initialAngle;
            
            // Debug.Log($"{gameObject.name}: 4방향 공격 컴포넌트가 초기화되었습니다. (회전 간격: {_rotationStep}도, 공격 간격: {_attackInterval}초)");
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
            
            int idx = Random.Range(0, enemyAttackSounds.Length); 
            enemyAttackSounds[idx].Play();
            for (int i = 0; i < 4; i++)
            {
                float shootAngle = _currentAngle + (i * 90f);
                
                float radians = shootAngle * Mathf.Deg2Rad;
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
            _enemy.transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);
            
            _currentAngle += _rotationStep;
            
            if (_currentAngle >= 360f)
            {
                _currentAngle -= 360f;
            }
            
            _lastAttackTime = Time.time;
            
            Debug.Log($"{gameObject.name}: 4방향 공격 실행! 기준 각도: {_currentAngle - _rotationStep:F1}도");
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < 4; i++)
            {
                float shootAngle = _currentAngle + (i * 90f);
                float radians = shootAngle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                );
                Vector3 endPoint = transform.position + (Vector3)(direction * 3f);
                Gizmos.DrawLine(transform.position, endPoint);
            }
            
            Gizmos.color = Color.gray;
            for (int i = 0; i < 4; i++)
            {
                float futureAngle = _currentAngle + _rotationStep + (i * 90f);
                float radians = futureAngle * Mathf.Deg2Rad;
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