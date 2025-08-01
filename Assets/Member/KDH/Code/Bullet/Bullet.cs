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

        private Vector2 _direction;
        private Camera _mainCamera;
        private float _spawnTime;
        private bool _isActive;
        private bool _isReflect;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("메인 카메라를 찾을 수 없습니다!");
            }
            originalPos = _mainCamera.transform.position;
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
            gameObject.SetActive(true);
        }

        private void MoveBullet()
        {
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
            if (Time.time - _spawnTime >= _lifeTime)
            {
                DestroyBullet();
            }
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
            gameObject.SetActive(false);

            BulletPool.Instance?.ReturnBullet(this);
        }

        public void SetReflect(bool isReflect)
        {
            if (isReflect)
                ShakeCamera();
            _isReflect = isReflect;
        }
    }
}