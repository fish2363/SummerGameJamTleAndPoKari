using System.Collections;
using Ami.BroAudio;
using DG.Tweening;
using Member.CUH.Code.Combat.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.KDH.Code.Bullet.AttackType
{
    public class RandomAttack : EnemyAttackCompo
    {
        [Header("무작위 공격 설정")]
        [SerializeField] private float _attackInterval = 0.8f; // 공격 간격 (초)
        [SerializeField] private float _bulletSpeed = 1f; // 탄환 속도
        [SerializeField] private int _bulletsPerAttack = 1; // 한 번 공격시 발사할 탄환 수
        [SerializeField] private bool _useRandomSeed = false; // 시드를 사용한 재현 가능한 랜덤 여부
        [SerializeField] private int _randomSeed = 12345; // 랜덤 시드 (재현 가능한 패턴용)
        [SerializeField] private bool _enableDebugLogs = true; // 디버그 로그 활성화 여부
        [SerializeField] private ParticleSystem teleportParticle;
        [SerializeField] private SoundID[] enemyAttackSounds;
        
        public float shakeDuration = 0.2f;
        public float shrinkDuration = 0.15f;
        public float scaleFactor = 0.6f;


        private float _lastAttackTime;
        private System.Random _randomGenerator;
        private bool _isInitialized = false;
        private Vector3 originalScale;
        Sequence seq;

        private void Awake()
        {
            SafeInitializeRandomGenerator();
            
            if (_enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] RandomAttack 컴포넌트가 Awake에서 초기화되었습니다.");
            }
        }
        
        public override void Initialize(Entity entity)
        {
            try
            {
                base.Initialize(entity);

                SafeInitializeRandomGenerator();
                
                _isInitialized = true;
                
                if (_enableDebugLogs)
                {
                    // Debug.Log($"[{gameObject.name}] 무작위 공격 컴포넌트가 성공적으로 초기화되었습니다. " +
                    //          $"(공격 간격: {_attackInterval}초, 탄환 수: {_bulletsPerAttack}발, " +
                    //          $"시드 사용: {_useRandomSeed})");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] RandomAttack 초기화 중 오류 발생: {ex.Message}");
                
                EnsureRandomGeneratorExists();
            }
            originalScale = _enemy.transform.localScale;
            SetTeleportSequence();
        }
        
        private void Update()
        {
            if (!_isInitialized)
            {
                SafeInitializeRandomGenerator();
                _isInitialized = true;
                
                if (_enableDebugLogs)
                {
                    Debug.LogWarning($"[{gameObject.name}] Update에서 늦은 초기화가 실행되었습니다.");
                }
            }
            
            // if (Time.time - _lastAttackTime >= _attackInterval)
            // {
            //     Attack();
            // }
        }
        
        private void SafeInitializeRandomGenerator()
        {
            try
            {
                if (_useRandomSeed)
                {
                    _randomGenerator = new System.Random(_randomSeed);
                    
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] 시드 {_randomSeed}를 사용한 재현 가능한 랜덤 패턴이 설정되었습니다.");
                    }
                }
                else
                {
                    int timeSeed = System.DateTime.Now.Millisecond + GetInstanceID();
                    _randomGenerator = new System.Random(timeSeed);
                    
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] 완전 랜덤 패턴이 설정되었습니다. (시드: {timeSeed})");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 랜덤 생성기 초기화 실패: {ex.Message}");

                EnsureRandomGeneratorExists();
            }
        }
        
        private void EnsureRandomGeneratorExists()
        {
            try
            {
                if (_randomGenerator == null)
                {
                    _randomGenerator = new System.Random();
                    
                    Debug.LogWarning($"[{gameObject.name}] 기본 랜덤 생성기로 대체되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 기본 랜덤 생성기 생성도 실패했습니다: {ex.Message}");
            }
        }
        
        public override void Attack()
        {
            try
            {
                base.Attack();
                
                if (BulletPool.Instance == null)
                {
                    Debug.LogError($"[{gameObject.name}] BulletPool이 준비되지 않았습니다! BulletPool 오브젝트가 씬에 있는지 확인하세요.");
                    return;
                }
                
                EnsureRandomGeneratorExists();
                
                if (_randomGenerator == null)
                {
                    Debug.LogError($"[{gameObject.name}] 랜덤 생성기를 초기화할 수 없습니다. 공격을 중단합니다.");
                    return;
                }
                
                int successfulShots = 0;
                
                StartCoroutine(ShoutCoroutine(successfulShots));
                
                _lastAttackTime = Time.time;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 무작위 공격 실행 완료! " +
                             $"성공: {successfulShots}발 / 시도: {_bulletsPerAttack}발");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] Attack 메서드 실행 중 오류 발생: {ex.Message}");
            }
        }

        private IEnumerator ShoutCoroutine(int successfulShots)
        {
            
            for (int i = 0; i < _bulletsPerAttack; i++)
            {
                try
                {
                    float randomAngle = GetRandomAngleSafe();
                        
                    float radians = randomAngle * Mathf.Deg2Rad;
                    Vector2 randomDirection = new Vector2(
                        Mathf.Cos(radians),
                        Mathf.Sin(radians)
                    );

                    Bullet bullet = BulletPool.Instance.GetBullet();
                    if (bullet == null)
                    {
                        Debug.LogWarning($"[{gameObject.name}] {i + 1}번째 탄환을 생성할 수 없습니다! 탄환 풀이 부족할 수 있습니다.");
                        continue;
                    }

                    bullet.transform.position = transform.position;
                    int idx = Random.Range(0, enemyAttackSounds.Length); 
                    enemyAttackSounds[idx].Play();
                    bullet.Fire(randomDirection, _bulletSpeed);
                        
                    successfulShots++;
                        
                    if (_enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] {i + 1}번째 탄환을 {randomAngle:F1}도 방향으로 발사했습니다.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[{gameObject.name}] {i + 1}번째 탄환 발사 중 오류 발생: {ex.Message}");
                }

                yield return new WaitForSeconds(_attackInterval);
            }
            TeleportAnimation();
            _nextPos.x = Random.Range(-_halfXSize, _halfXSize);
            _nextPos.y = Random.Range(-_halfYSize, _halfYSize);
            _enemy.transform.position = _nextPos;
        }

        private float GetRandomAngleSafe()
        {
            try
            {
                if (_randomGenerator == null)
                {
                    Debug.LogWarning($"[{gameObject.name}] 랜덤 생성기가 null입니다. 재초기화를 시도합니다.");
                    EnsureRandomGeneratorExists();
                    
                    if (_randomGenerator == null)
                    {
                        Debug.LogWarning($"[{gameObject.name}] Unity의 기본 Random을 사용합니다.");
                        return UnityEngine.Random.Range(0f, 360f);
                    }
                }
                
                double randomValue = _randomGenerator.NextDouble();
                
                float angle = (float)(randomValue * 360.0);
                
                return angle;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 랜덤 각도 생성 중 오류 발생: {ex.Message}");
                
                return UnityEngine.Random.Range(0f, 360f);
            }
        }
        
        public void ChangeRandomSeed(int newSeed)
        {
            try
            {
                _randomSeed = newSeed;
                _useRandomSeed = true;
                SafeInitializeRandomGenerator();
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 랜덤 시드가 {newSeed}로 변경되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 랜덤 시드 변경 중 오류 발생: {ex.Message}");
            }
        }
        
        public void EnableTrueRandom()
        {
            try
            {
                _useRandomSeed = false;
                SafeInitializeRandomGenerator();
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] 완전 랜덤 모드로 전환되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[{gameObject.name}] 완전 랜덤 모드 전환 중 오류 발생: {ex.Message}");
            }
        }

        public void SetDebugLogging(bool enable)
        {
            _useRandomSeed = enable;
            
            if (enable)
            {
                Debug.Log($"[{gameObject.name}] 디버그 로그가 활성화되었습니다.");
            }
        }
        
        public bool IsInitialized()
        {
            return _isInitialized && _randomGenerator != null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsInitialized() ? Color.magenta : Color.red;
            Gizmos.DrawWireSphere(transform.position, 2f);

            Gizmos.color = new Color(1f, 0f, 1f, IsInitialized() ? 0.1f : 0.05f);
            Gizmos.DrawSphere(transform.position, 2f);
            
            Gizmos.color = IsInitialized() ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
        }
        public void TeleportAnimation()
        {
            if (seq.IsActive()) seq.Restart(); // 매번 새로 만들지 않고 재시작
        }

        private float _halfXSize = 4.5f;
        private float _halfYSize = 4.5f;
        private Vector2 _nextPos;

        private void SetTeleportSequence()
        {
            seq = DOTween.Sequence();

            seq.Append(transform.DOShakePosition(shakeDuration, 0.2f))
           .Join(transform.DOScale(originalScale * scaleFactor, shrinkDuration))
           .Append(transform.DOScale(originalScale, 0.2f))
           .SetEase(Ease.InOutQuad);
        }

        private void OnEnable()
        {
            if (!IsInitialized())
            {
                SafeInitializeRandomGenerator();
                _isInitialized = true;
                
                if (_enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] OnEnable에서 초기화가 실행되었습니다.");
                }
            }
        }

        private void OnDestroy()
        {
            _randomGenerator = null;
            _isInitialized = false;
            
            if (_enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] RandomAttack 컴포넌트가 정리되었습니다.");
            }
        }
    }
}