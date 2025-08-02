using DG.Tweening;
using Member.CUH.Code.Combat;
using UnityEngine;

namespace Member.KDH.Code.Bullet
{
    public class Bullet : MonoBehaviour
    {
        [Header("탄환 설정")]
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _lifeTime = 10f;
        [SerializeField] private Color reflectColor = Color.blue;
        [SerializeField] private float reflectMinSpeed = 5f;
        
        [Header("깜빡거리기 설정")]
        [SerializeField] private float blinkSpeed = 30f; // 깜빡거리는 속도
        [SerializeField] private float minAlpha = 0.001f; // 최소 알파값
        
        private Vector2 _direction;
        private Camera _mainCamera;
        private float _spawnTime;
        private bool _isActive;
        private bool _isReflect;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private bool _isBlinking;

        public static bool isSlowy;
        public static bool isFaster;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("메인 카메라를 찾을 수 없습니다!");
            }
            originalPos = _mainCamera.transform.position;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Debug.Assert(_spriteRenderer != null, "_spriteRenderer != null");
            _spriteRenderer.color = Color.red;
            _originalColor = _spriteRenderer.color;
        }

        private void Update()
        {
            if (!_isActive) return;

            MoveBullet();

            CheckScreenBoundary();

            CheckLifeTime();
        }

        public void Fire(Vector2 direction, float speed = 0f)
        {
            _direction = direction.normalized;
            if (speed > 0f)
            {
                _speed = speed;
            }

            _spawnTime = Time.time;
            _isActive = true;
            _isBlinking = false;
            
            // 색상 초기화
            if (_isReflect)
                _originalColor = reflectColor;
            else
                _originalColor = Color.red;
                
            _spriteRenderer.color = _originalColor;
            gameObject.SetActive(true);
        }

        private void MoveBullet()
        {
            if(isSlowy)
                transform.Translate(_direction * (_speed / 4f) * Time.deltaTime);
            else if (isFaster)
                transform.Translate(_direction * (_speed * 1.3f) * Time.deltaTime);
            else
                transform.Translate(_direction * _speed * Time.deltaTime);
        }

        private void CheckScreenBoundary()
        {
            if (_mainCamera == null) return;

            Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(transform.position);
            Vector3 newPosition = transform.position;
            bool teleported = false;

            if (viewportPosition.x < 0f)
            {
                viewportPosition.x = 1f;
                teleported = true;
            }
            else if (viewportPosition.x > 1f)
            {
                viewportPosition.x = 0f;
                teleported = true;
            }

            if (viewportPosition.y < 0f)
            {
                viewportPosition.y = 1f;
                teleported = true;
            }
            else if (viewportPosition.y > 1f)
            {
                viewportPosition.y = 0f;
                teleported = true;
            }

            if (teleported)
            {
                newPosition = _mainCamera.ViewportToWorldPoint(viewportPosition);
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
        }

        private void CheckLifeTime()
        {
            float elapsedTime = Time.time - _spawnTime;
            float remainingTime = _lifeTime - elapsedTime;
            
            // 남은 시간이 3초 이하일 때 깜빡거리기 시작
            if (remainingTime <= 3f && remainingTime > 0f && !_isBlinking)
            {
                StartBlinking();
            }
            
            // 깜빡거리는 중일 때 알파값 조정
            if (_isBlinking && remainingTime > 0f)
            {
                float alpha = Mathf.Lerp(minAlpha, 1f, (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f);
                Color currentColor = _originalColor;
                currentColor.a = alpha;
                _spriteRenderer.color = currentColor;
            }
            
            if (remainingTime <= 0f)
            {
                DestroyBullet();
            }
        }

        private void StartBlinking()
        {
            _isBlinking = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isReflect)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (other.TryGetComponent(out IDamageable damageable))
                        damageable.ApplyDamage(1);
                    Debug.Log("Hit! Enemy");
                    DestroyBullet();
                }
            }
            else
            {
                if (other.CompareTag("Player"))
                {
                    if (other.TryGetComponent(out IDamageable damageable))
                        damageable.ApplyDamage(1);
                    Debug.Log("Hit! Player");
                    DestroyBullet();
                }
            }

        }
        float shakeDuration = 0.1f;
        float shakeStrength = 0.1f;

        private Vector3 originalPos;
        public void ShakeCamera()
        {
            Camera.main.transform.localPosition = originalPos; // 혹시 이전 흔들림에서 안 돌아왔으면 리셋

            Camera.main.transform
                .DOShakePosition(shakeDuration, shakeStrength, vibrato: 5, randomness: 10, snapping: false, fadeOut: true)
                .OnComplete(() => Camera.main.transform.localPosition = originalPos); // 흔들고 난 뒤 원위치 보정
        }
        public void DestroyBullet()
        {
            _isReflect = false;
            _isActive = false;
            _isBlinking = false;
            
            // 색상 원래대로 복구
            Color resetColor = _originalColor;
            resetColor.a = 1f;
            _spriteRenderer.color = resetColor;
            
            gameObject.SetActive(false);

            BulletPool.Instance?.ReturnBullet(this);
        }

        public void SetReflect(bool isReflect)
        {
            if (isReflect)
            {
                ShakeCamera();
                _spriteRenderer.color = reflectColor;
                _originalColor = reflectColor;
                _speed = _speed > reflectMinSpeed ? _speed : reflectMinSpeed;
            }
            else
            {
                _originalColor = Color.red;
            }
            _isReflect = isReflect;
        }
        
        public Vector2 GetDirection() => _direction;
    }
}