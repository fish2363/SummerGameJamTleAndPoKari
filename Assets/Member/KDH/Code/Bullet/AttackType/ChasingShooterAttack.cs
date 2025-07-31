using Member.CUH.Code.Entities;
using Member.KDH.Code.Bullet;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChasingShooterAttack : EnemyAttackCompo
    {
        [Header("추적 이동 설정")]
        [SerializeField] private float _maxSpeed = 2f; // 최대 이동 속도 (더 느림)
        [SerializeField] private float _acceleration = 1.5f; // 가속도
        
        [Header("공격 설정")]
        [SerializeField] private float _attackInterval = 1f; // 공격 간격 (초)
        [SerializeField] private float _bulletSpeed = 1f; // 탄환 속도
        
        [Header("디버그 설정")]
        [SerializeField] private bool _enableDebugLogs = true; // 디버그 로그 활성화
        [SerializeField] private bool _showGizmos = true; // 기즈모 표시 여부
        
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody;
        private Vector2 _currentVelocity;
        private float _lastAttackTime;
        private bool _isInitialized = false;
        private bool _playerFound = false;
        private float _lastPlayerSearchTime = 0f;
        private readonly float _playerSearchInterval = 1f; // 플레이어 재탐지 간격
        
        private void Awake()
        {
            try
            {
                InitializeRigidbody();
                PerformBasicInitialization();
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] ChasingShooterAttack이 Awake에서 초기화되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Awake 초기화 중 오류 발생: {ex.Message}");
            }
        }
        
        private void Start()
        {
            try
            {
                FindPlayer();
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] Start에서 추가 초기화가 완료되었습니다. 플레이어 발견: {_playerFound}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Start 초기화 중 오류 발생: {ex.Message}");
            }
        }
        
        public override void Initialize(Entity entity)
        {
            try
            {
                base.Initialize(entity);
                
                if (_rigidbody == null)
                {
                    InitializeRigidbody();
                }
                
                if (!_playerFound)
                {
                    FindPlayer();
                }
                
                _isInitialized = true;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] ChasingShooterAttack이 성공적으로 초기화되었습니다. " +
                             $"(최대속도: {_maxSpeed}, 가속도: {_acceleration}, 플레이어 발견: {_playerFound})");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Initialize 메서드 실행 중 오류 발생: {ex.Message}");
                PerformBasicInitialization();
                _isInitialized = true;
            }
        }
        
        private void Update()
        {
            try
            {
                if (!_isInitialized)
                {
                    PerformBasicInitialization();
                    _isInitialized = true;
                    
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] Update에서 늦은 초기화가 실행되었습니다.");
                    }
                }
                
                if (!_playerFound && Time.time - _lastPlayerSearchTime >= _playerSearchInterval)
                {
                    FindPlayer();
                    _lastPlayerSearchTime = Time.time;
                }
                
                if (_playerFound && _playerTransform != null)
                {
                    ChasePlayer();
                }
                
                if (Time.time - _lastAttackTime >= _attackInterval)
                {
                    Attack();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Update 실행 중 오류 발생: {ex.Message}");
            }
        }
        
        private void InitializeRigidbody()
        {
            try
            {
                _rigidbody = GetComponent<Rigidbody2D>();
                
                if (_rigidbody == null)
                {
                    Debug.LogError($"[{gameObject.name}] Rigidbody2D 컴포넌트가 필요합니다!");
                    _rigidbody = gameObject.AddComponent<Rigidbody2D>();
                    Debug.LogWarning($"[{gameObject.name}] Rigidbody2D 컴포넌트를 자동으로 추가했습니다.");
                }
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] Rigidbody2D 컴포넌트가 성공적으로 초기화되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Rigidbody2D 초기화 중 오류 발생: {ex.Message}");
            }
        }
        
        private void PerformBasicInitialization()
        {
            try
            {
                _currentVelocity = Vector2.zero;
                _playerFound = false;
                _lastPlayerSearchTime = 0f;
                _lastAttackTime = 0f;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 기본 초기화가 완료되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 기본 초기화 중 오류 발생: {ex.Message}");
            }
        }
        
        private void FindPlayer()
        {
            try
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                
                if (player != null)
                {
                    _playerTransform = player.transform;
                    _playerFound = true;
                    
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] 플레이어를 성공적으로 찾았습니다. " +
                                 $"플레이어 위치: {_playerTransform.position}");
                    }
                }
                else
                {
                    _playerTransform = null;
                    _playerFound = false;
                    
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 플레이어를 찾을 수 없습니다! " +
                                        "Player 태그가 올바르게 설정되었는지 확인해주세요.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 플레이어 탐지 중 오류 발생: {ex.Message}");
                _playerFound = false;
                _playerTransform = null;
            }
        }
        
        private void ChasePlayer()
        {
            try
            {
                if (_rigidbody == null || _playerTransform == null)
                {
                    if (_enableDebugLogs && Time.frameCount % 120 == 0)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 추적에 필요한 컴포넌트가 준비되지 않았습니다. " +
                                        $"Rigidbody2D: {_rigidbody != null}, Player: {_playerTransform != null}");
                    }
                    return;
                }
                
                Vector2 directionToPlayer = (_playerTransform.position - transform.position).normalized;
                float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
                Vector2 targetVelocity = directionToPlayer * _maxSpeed;
                
                _currentVelocity = Vector2.MoveTowards(
                    _currentVelocity, 
                    targetVelocity, 
                    _acceleration * Time.deltaTime
                );
                
                _rigidbody.linearVelocity = _currentVelocity;
                
                if (_enableDebugLogs && Time.frameCount % 60 == 0)
                {
                    Debug.Log($"[{gameObject.name}] 추적 중 - 거리: {distanceToPlayer:F2}, " +
                             $"현재속도: {_currentVelocity.magnitude:F2}, 방향: {directionToPlayer}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 플레이어 추적 중 오류 발생: {ex.Message}");
            }
        }
        
        public override void Attack()
        {
            try
            {
                base.Attack();
                
                if (_playerTransform == null || BulletPool.Instance == null) 
                {
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 공격에 필요한 컴포넌트가 준비되지 않았습니다. " +
                                        $"Player: {_playerTransform != null}, BulletPool: {BulletPool.Instance != null}");
                    }
                    return;
                }
                
                Vector2 shootDirection = (_playerTransform.position - transform.position).normalized;
                
                Bullet bullet = BulletPool.Instance.GetBullet();
                if (bullet == null)
                {
                    Debug.LogError($"[{gameObject.name}] 탄환을 생성할 수 없습니다!");
                    return;
                }
                
                bullet.transform.position = transform.position;
                bullet.Fire(shootDirection, _bulletSpeed);
                
                _lastAttackTime = Time.time;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 탄환 발사! 방향: {shootDirection}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Attack 메서드 실행 중 오류 발생: {ex.Message}");
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            try
            {
                if (other.CompareTag("Player"))
                {
                    Debug.Log($"[{gameObject.name}] 플레이어와 충돌! Hit!");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 충돌 처리 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetMaxSpeed(float newMaxSpeed)
        {
            try
            {
                if (newMaxSpeed < 0f)
                {
                    Debug.LogWarning($"[{gameObject.name}] 최대 속도는 0 이상이어야 합니다. 입력값: {newMaxSpeed}");
                    return;
                }
                
                _maxSpeed = newMaxSpeed;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 최대 속도가 {newMaxSpeed}로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 최대 속도 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetAcceleration(float newAcceleration)
        {
            try
            {
                if (newAcceleration < 0f)
                {
                    Debug.LogWarning($"[{gameObject.name}] 가속도는 0 이상이어야 합니다. 입력값: {newAcceleration}");
                    return;
                }
                
                _acceleration = newAcceleration;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 가속도가 {newAcceleration}로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 가속도 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetAttackInterval(float newInterval)
        {
            try
            {
                if (newInterval <= 0f)
                {
                    Debug.LogWarning($"[{gameObject.name}] 공격 간격은 0보다 커야 합니다. 입력값: {newInterval}");
                    return;
                }
                
                _attackInterval = newInterval;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 공격 간격이 {newInterval}초로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 공격 간격 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetBulletSpeed(float newSpeed)
        {
            try
            {
                if (newSpeed <= 0f)
                {
                    Debug.LogWarning($"[{gameObject.name}] 탄환 속도는 0보다 커야 합니다. 입력값: {newSpeed}");
                    return;
                }
                
                _bulletSpeed = newSpeed;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 탄환 속도가 {newSpeed}로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 탄환 속도 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetDebugLogging(bool enable)
        {
            _enableDebugLogs = enable;
            
            if (enable)
            {
                Debug.Log($"[{gameObject.name}] 디버그 로그가 활성화되었습니다.");
            }
        }
        
        public bool IsReadyToChase()
        {
            return _isInitialized && _playerFound && _rigidbody != null && _playerTransform != null;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!_showGizmos) return;
            
            try
            {
                Color gizmosColor = IsReadyToChase() ? Color.green : Color.red;
                
                Gizmos.color = gizmosColor;
                Gizmos.DrawWireSphere(transform.position, 1f);
                
                if (_playerFound && _playerTransform != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, _playerTransform.position);
                    
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(_playerTransform.position, 0.5f);
                    
                    if (_rigidbody != null && _rigidbody.linearVelocity.magnitude > 0.1f)
                    {
                        Gizmos.color = Color.yellow;
                        Vector3 velocityEnd = transform.position + (Vector3)_rigidbody.linearVelocity;
                        Gizmos.DrawLine(transform.position, velocityEnd);
                    }
                }
                
                Gizmos.color = IsReadyToChase() ? Color.green : Color.red;
                Gizmos.DrawWireCube(transform.position, Vector3.one * 0.3f);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 기즈모 그리기 중 오류 발생: {ex.Message}");
            }
        }
        
        private void OnEnable()
        {
            try
            {
                if (!_isInitialized)
                {
                    PerformBasicInitialization();
                    _isInitialized = true;
                }
                
                if (!_playerFound)
                {
                    FindPlayer();
                }
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] OnEnable에서 초기화가 실행되었습니다. " +
                             $"준비 상태: {IsReadyToChase()}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] OnEnable 처리 중 오류 발생: {ex.Message}");
            }
        }
        
        private void OnDestroy()
        {
            try
            {
                _playerTransform = null;
                _rigidbody = null;
                _isInitialized = false;
                _playerFound = false;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] ChasingShooterAttack 컴포넌트가 정리되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] OnDestroy 처리 중 오류 발생: {ex.Message}");
            }
        }
    }
}