using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class PredictiveAttack : EnemyAttackCompo
    {
        [Header("예측 공격 설정")]
        // [SerializeField] private float _attackInterval = 2f; // 공격 간격 (초)
        [SerializeField] private float _bulletSpeed = 3f; // 고속탄 속도 (기본보다 빠름)
        [SerializeField] private float _predictionTime = 1f; // 예측 시간 (초) - 얼마나 미래를 예측할지
        [SerializeField] private bool _drawPredictionGizmo = true; // 예측 지점 시각화 여부
        
        private Transform _playerTransform;
        private Rigidbody2D _playerRigidbody;
        private float _lastAttackTime;
        private Vector3 _predictedPosition;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            FindPlayerComponents();
            
            // Debug.Log($"{gameObject.name}: 예측 공격 컴포넌트가 초기화되었습니다. (공격 간격: {_attackInterval}초, 예측 시간: {_predictionTime}초)");
        }
        
        private void Update()
        {
            if (_playerTransform == null || _playerRigidbody == null)
            {
                FindPlayerComponents();
                return;
            }
            
            // if (Time.time - _lastAttackTime >= _attackInterval)
            // {
            //     Attack();
            // }
        }
        
        private void FindPlayerComponents()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
                _playerRigidbody = player.GetComponent<Rigidbody2D>();
                
                if (_playerRigidbody == null)
                {
                    Debug.LogError($"{gameObject.name}: 플레이어에게 Rigidbody2D 컴포넌트가 없습니다! 예측 공격을 위해서는 Rigidbody2D가 필요합니다.");
                }
                else
                {
                    Debug.Log($"{gameObject.name}: 플레이어 컴포넌트를 성공적으로 찾았습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: 플레이어를 찾을 수 없습니다! Player 태그를 확인해주세요.");
            }
        }
        
        public override void Attack()
        {
            base.Attack();
            
            if (_playerTransform == null || _playerRigidbody == null || BulletPool.Instance == null)
            {
                Debug.LogWarning($"{gameObject.name}: 예측 공격에 필요한 컴포넌트가 준비되지 않았습니다.");
                return;
            }
            
            Vector3 predictedPlayerPosition = CalculatePredictedPosition();
            
            Vector2 shootDirection = (predictedPlayerPosition - transform.position).normalized;
            
            Bullet bullet = BulletPool.Instance.GetBullet();
            if (bullet == null)
            {
                Debug.LogError($"{gameObject.name}: 고속탄을 생성할 수 없습니다!");
                return;
            }
            
            bullet.transform.position = transform.position;
            
            bullet.Fire(shootDirection, _bulletSpeed);
            
            _lastAttackTime = Time.time;
            
            _predictedPosition = predictedPlayerPosition;
            
            Debug.Log($"{gameObject.name}: 예측 공격 실행! 예측 위치: {predictedPlayerPosition}, 현재 플레이어 위치: {_playerTransform.position}");
        }
        
        private Vector3 CalculatePredictedPosition()
        {
            Vector3 currentPosition = _playerTransform.position;
            Vector2 currentVelocity = _playerRigidbody.linearVelocity;

            float distanceToPlayer = Vector3.Distance(transform.position, currentPosition);
            float timeToReachPlayer = distanceToPlayer / _bulletSpeed;

            float actualPredictionTime = Mathf.Min(_predictionTime, timeToReachPlayer);
            
            Vector3 predictedPosition = currentPosition + (Vector3)(currentVelocity * actualPredictionTime);
            
            return predictedPosition;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!_drawPredictionGizmo) return;
            
            if (_playerTransform != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_playerTransform.position, 0.3f);
                
                if (_predictedPosition != Vector3.zero)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(_predictedPosition, 0.4f);
                    
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, _predictedPosition);
                }
                
                if (_playerRigidbody != null && _playerRigidbody.linearVelocity.magnitude > 0.1f)
                {
                    Gizmos.color = Color.blue;
                    Vector3 velocityEnd = _playerTransform.position + (Vector3)_playerRigidbody.linearVelocity;
                    Gizmos.DrawLine(_playerTransform.position, velocityEnd);
                }
            }
        }
    }
}