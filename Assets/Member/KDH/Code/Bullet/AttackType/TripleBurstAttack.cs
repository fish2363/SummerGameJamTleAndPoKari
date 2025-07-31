using System.Collections;
using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class TripleBurstAttack : EnemyAttackCompo
    {
        [SerializeField] private int _burstCount = 3;
        [SerializeField] private float _burstDelay = 0.1f;
        [SerializeField] private float _burstCooldown = 2f;
        [SerializeField] private float _bulletSpeed = 3.5f;
        [SerializeField] private bool _useFixedDirection = false;
        [SerializeField] private bool _enableDebugLogs = true;
        
        private enum AttackState
        {
            Waiting,
            Bursting
        }
        
        private Transform _playerTransform;
        private AttackState _currentState = AttackState.Waiting;
        private Vector2 _fixedShootDirection;
        private float _lastBurstTime;
        private bool _isInitialized = false;
        private bool _playerFound = false;
        private float _lastPlayerSearchTime = 0f;
        private readonly float _playerSearchInterval = 1f;
        private Coroutine _burstCoroutine;
        
        private void Awake()
        {
            try
            {
                PerformBasicInitialization();
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] TripleBurstAttack이 Awake에서 초기화되었습니다.");
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
                    Debug.Log($"[{gameObject.name}] Start에서 초기화가 완료되었습니다. 플레이어 발견: {_playerFound}");
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
                
                if (!_playerFound)
                {
                    FindPlayer();
                }
                
                _isInitialized = true;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] TripleBurstAttack이 성공적으로 초기화되었습니다. " +
                             $"(점사수: {_burstCount}발, 딜레이: {_burstDelay}초, 쿨타임: {_burstCooldown}초, 플레이어 발견: {_playerFound})");
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
                
                CheckAndExecuteBurstAttack();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Update 실행 중 오류 발생: {ex.Message}");
            }
        }
        
        private void PerformBasicInitialization()
        {
            try
            {
                _currentState = AttackState.Waiting;
                _fixedShootDirection = Vector2.down;
                _lastBurstTime = 0f;
                _playerFound = false;
                _lastPlayerSearchTime = 0f;
                _burstCoroutine = null;
                
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
        
        private void CheckAndExecuteBurstAttack()
        {
            try
            {
                if (_currentState == AttackState.Waiting && 
                    Time.time - _lastBurstTime >= _burstCooldown)
                {
                    StartBurstAttack();
                }
                
                if (_enableDebugLogs && Time.frameCount % 300 == 0)
                {
                    float remainingCooldown = _burstCooldown - (Time.time - _lastBurstTime);
                    Debug.Log($"[{gameObject.name}] 현재 상태: {GetStateKoreanName()}, " +
                             $"쿨타임 남은 시간: {Mathf.Max(0f, remainingCooldown):F1}초");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 점사 공격 타이밍 체크 중 오류 발생: {ex.Message}");
            }
        }
        
        private string GetStateKoreanName()
        {
            return _currentState switch
            {
                AttackState.Waiting => "대기 중",
                AttackState.Bursting => "점사 중",
                _ => "알 수 없음"
            };
        }
        
        private void StartBurstAttack()
        {
            try
            {
                if (_currentState == AttackState.Bursting || _burstCoroutine != null)
                {
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 이미 점사 중이므로 새로운 점사를 시작할 수 없습니다.");
                    }
                    return;
                }
                
                if (!ValidateBurstPrerequisites())
                {
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 점사 공격 조건이 충족되지 않았습니다.");
                    }
                    return;
                }
                
                _currentState = AttackState.Bursting;
                
                if (_useFixedDirection && _playerTransform != null)
                {
                    _fixedShootDirection = (_playerTransform.position - transform.position).normalized;
                    
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] 고정 방향 모드: 발사 방향을 {_fixedShootDirection}로 고정했습니다.");
                    }
                }
                
                _burstCoroutine = StartCoroutine(BurstFireCoroutine());
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 점사 공격 시작! " +
                             $"(고정방향모드: {_useFixedDirection})");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 점사 공격 시작 중 오류 발생: {ex.Message}");
                
                _currentState = AttackState.Waiting;
                _burstCoroutine = null;
            }
        }
        
        private bool ValidateBurstPrerequisites()
        {
            if (BulletPool.Instance == null)
            {
                if (_enableDebugLogs)
                {
                    Debug.LogError($"[{gameObject.name}] BulletPool이 준비되지 않았습니다!");
                }
                return false;
            }
            
            if (_burstCount <= 0)
            {
                if (_enableDebugLogs)
                {
                    Debug.LogError($"[{gameObject.name}] 잘못된 점사 개수: {_burstCount}");
                }
                return false;
            }
            
            if (_burstDelay < 0f)
            {
                if (_enableDebugLogs)
                {
                    Debug.LogError($"[{gameObject.name}] 잘못된 점사 딜레이: {_burstDelay}");
                }
                return false;
            }
            
            if (_bulletSpeed <= 0f)
            {
                if (_enableDebugLogs)
                {
                    Debug.LogError($"[{gameObject.name}] 잘못된 탄환 속도: {_bulletSpeed}");
                }
                return false;
            }
            
            return true;
        }
        
        private IEnumerator BurstFireCoroutine()
        {
            int successfulShots = 0;
            
            for (int i = 0; i < _burstCount; i++)
            {
                if (!IsValidForShooting())
                {
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 점사 {i + 1}/{_burstCount}발 - 발사 조건 불충족으로 건너뜀");
                    }
                    
                    if (i < _burstCount - 1)
                    {
                        yield return new WaitForSeconds(_burstDelay);
                    }
                    continue;
                }
                
                Vector2 shootDirection = GetCurrentShootDirection();
                
                if (shootDirection.magnitude < 0.1f)
                {
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 점사 {i + 1}/{_burstCount}발 - 유효하지 않은 발사 방향");
                    }
                    
                    if (i < _burstCount - 1)
                    {
                        yield return new WaitForSeconds(_burstDelay);
                    }
                    continue;
                }
                
                bool shotResult = FireBulletSafely(shootDirection);
                
                if (shotResult)
                {
                    successfulShots++;
                    
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] 점사 {i + 1}/{_burstCount}발 발사 성공! " +
                                 $"방향: {shootDirection}, 속도: {_bulletSpeed}");
                    }
                }
                else
                {
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 점사 {i + 1}/{_burstCount}발 발사 실패!");
                    }
                }
                
                if (i < _burstCount - 1)
                {
                    yield return new WaitForSeconds(_burstDelay);
                }
            }
            
            CompleteBurstAttack(successfulShots);
        }
        
        private bool IsValidForShooting()
        {
            if (BulletPool.Instance == null)
            {
                return false;
            }
            
            if (!gameObject.activeInHierarchy || !enabled)
            {
                return false;
            }
            
            if (_currentState != AttackState.Bursting)
            {
                return false;
            }
            
            return true;
        }
        
        private bool FireBulletSafely(Vector2 direction)
        {
            if (BulletPool.Instance == null)
            {
                return false;
            }
            
            Bullet bullet = null;
            
            try
            {
                bullet = BulletPool.Instance.GetBullet();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 탄환 풀에서 탄환 가져오는 중 오류 발생: {ex.Message}");
                return false;
            }
            
            if (bullet == null)
            {
                if (_enableDebugLogs)
                {
                    Debug.LogWarning($"[{gameObject.name}] 탄환을 생성할 수 없습니다! 탄환 풀이 부족할 수 있습니다.");
                }
                return false;
            }
            
            try
            {
                bullet.transform.position = transform.position;
                bullet.Fire(direction, _bulletSpeed);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 탄환 발사 설정 중 오류 발생: {ex.Message}");
                
                try
                {
                    if (bullet != null && BulletPool.Instance != null)
                    {
                        BulletPool.Instance.ReturnBullet(bullet);
                    }
                }
                catch (System.Exception returnEx)
                {
                    Debug.LogError($"[{gameObject.name}] 실패한 탄환 반환 중 오류 발생: {returnEx.Message}");
                }
                
                return false;
            }
        }
        
        private void CompleteBurstAttack(int successfulShots)
        {
            try
            {
                _lastBurstTime = Time.time;
                _currentState = AttackState.Waiting;
                _burstCoroutine = null;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 점사 완료! 성공: {successfulShots}/{_burstCount}발, " +
                             $"다음 점사까지 {_burstCooldown}초 대기");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 점사 완료 처리 중 오류 발생: {ex.Message}");
                
                _currentState = AttackState.Waiting;
                _burstCoroutine = null;
            }
        }
        
        private Vector2 GetCurrentShootDirection()
        {
            try
            {
                if (_useFixedDirection)
                {
                    return _fixedShootDirection;
                }
                else
                {
                    if (_playerTransform != null)
                    {
                        Vector2 direction = (_playerTransform.position - transform.position).normalized;
                        return direction;
                    }
                    else
                    {
                        if (_enableDebugLogs)
                        {
                            Debug.LogWarning($"[{gameObject.name}] 플레이어를 찾을 수 없어 기본 방향(아래쪽)으로 발사합니다.");
                        }
                        return Vector2.down;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 발사 방향 계산 중 오류 발생: {ex.Message}");
                return Vector2.down;
            }
        }
        
        public override void Attack()
        {
            try
            {
                base.Attack();
                
                if (_currentState == AttackState.Waiting && 
                    Time.time - _lastBurstTime >= _burstCooldown)
                {
                    StartBurstAttack();
                    
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] 외부에서 Attack 호출로 점사 시작!");
                    }
                }
                else
                {
                    if (_enableDebugLogs)
                    {
                        float remainingCooldown = _burstCooldown - (Time.time - _lastBurstTime);
                        Debug.LogWarning($"[{gameObject.name}] 점사 공격을 할 수 없는 상태입니다. " +
                                        $"(상태: {GetStateKoreanName()}, 쿨타임 남은 시간: {Mathf.Max(0f, remainingCooldown):F1}초)");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Attack 메서드 실행 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetBurstCount(int newBurstCount)
        {
            try
            {
                if (newBurstCount <= 0)
                {
                    Debug.LogWarning($"[{gameObject.name}] 점사 개수는 1 이상이어야 합니다. 입력값: {newBurstCount}");
                    return;
                }
                
                _burstCount = newBurstCount;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 점사 개수가 {newBurstCount}발로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 점사 개수 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetBurstDelay(float newBurstDelay)
        {
            try
            {
                if (newBurstDelay < 0f)
                {
                    Debug.LogWarning($"[{gameObject.name}] 점사 딜레이는 0 이상이어야 합니다. 입력값: {newBurstDelay}");
                    return;
                }
                
                _burstDelay = newBurstDelay;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 점사 딜레이가 {newBurstDelay}초로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 점사 딜레이 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetBurstCooldown(float newBurstCooldown)
        {
            try
            {
                if (newBurstCooldown < 0f)
                {
                    Debug.LogWarning($"[{gameObject.name}] 점사 쿨타임은 0 이상이어야 합니다. 입력값: {newBurstCooldown}");
                    return;
                }
                
                _burstCooldown = newBurstCooldown;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 점사 쿨타임이 {newBurstCooldown}초로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 점사 쿨타임 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetBulletSpeed(float newBulletSpeed)
        {
            try
            {
                if (newBulletSpeed <= 0f)
                {
                    Debug.LogWarning($"[{gameObject.name}] 탄환 속도는 0보다 커야 합니다. 입력값: {newBulletSpeed}");
                    return;
                }
                
                _bulletSpeed = newBulletSpeed;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 탄환 속도가 {newBulletSpeed}로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 탄환 속도 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void SetFixedDirectionMode(bool useFixed)
        {
            try
            {
                _useFixedDirection = useFixed;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 발사 방향 모드가 " +
                             $"{(useFixed ? "고정 방향" : "실시간 추적")} 모드로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 발사 방향 모드 변경 중 오류 발생: {ex.Message}");
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
        
        public bool IsReadyToAttack()
        {
            return _isInitialized && _playerFound && _playerTransform != null;
        }
        
        public string GetCurrentStateInfo()
        {
            try
            {
                string stateKorean = GetStateKoreanName();
                float remainingCooldown = _burstCooldown - (Time.time - _lastBurstTime);
                
                if (_currentState == AttackState.Waiting)
                {
                    return $"{stateKorean} (쿨타임 남은 시간: {Mathf.Max(0f, remainingCooldown):F1}초)";
                }
                else
                {
                    return $"{stateKorean} (진행 중)";
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 상태 정보 조회 중 오류 발생: {ex.Message}");
                return "오류 발생";
            }
        }
        
        public bool IsBursting()
        {
            return _currentState == AttackState.Bursting && _burstCoroutine != null;
        }
        
        public float GetRemainingCooldown()
        {
            return Mathf.Max(0f, _burstCooldown - (Time.time - _lastBurstTime));
        }
        
        public void ForceStartBurst()
        {
            try
            {
                if (_currentState == AttackState.Bursting)
                {
                    if (_enableDebugLogs)
                    {
                        Debug.LogWarning($"[{gameObject.name}] 이미 점사 중이므로 강제 점사를 시작할 수 없습니다.");
                    }
                    return;
                }
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 강제 점사 시작! (쿨타임 무시)");
                }
                
                StartBurstAttack();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 강제 점사 시작 중 오류 발생: {ex.Message}");
            }
        }
        
        public void StopBurst()
        {
            try
            {
                if (_burstCoroutine != null)
                {
                    StopCoroutine(_burstCoroutine);
                    _burstCoroutine = null;
                    
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] 진행 중인 점사가 중단되었습니다.");
                    }
                }
                
                _currentState = AttackState.Waiting;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 점사 중단 중 오류 발생: {ex.Message}");
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
                             $"준비 상태: {IsReadyToAttack()}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] OnEnable 처리 중 오류 발생: {ex.Message}");
            }
        }
        
        private void OnDisable()
        {
            try
            {
                if (_burstCoroutine != null)
                {
                    StopCoroutine(_burstCoroutine);
                    _burstCoroutine = null;
                    
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] OnDisable에서 진행 중인 점사 코루틴을 중지했습니다.");
                    }
                }
                
                _currentState = AttackState.Waiting;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] OnDisable 처리 중 오류 발생: {ex.Message}");
            }
        }
        
        private void OnDestroy()
        {
            try
            {
                if (_burstCoroutine != null)
                {
                    StopCoroutine(_burstCoroutine);
                    _burstCoroutine = null;
                }
                
                _playerTransform = null;
                _isInitialized = false;
                _playerFound = false;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] TripleBurstAttack 컴포넌트가 정리되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] OnDestroy 처리 중 오류 발생: {ex.Message}");
            }
        }
    }
}