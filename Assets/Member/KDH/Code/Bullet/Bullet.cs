using Member.CUH.Code.Combat;
using UnityEngine;

namespace Member.KDH.Code.Bullet
{
    public class Bullet : MonoBehaviour
    {
        [Header("탄환 설정")] [SerializeField] private float _speed = 1f;
        [SerializeField] private float _lifeTime = 10f;
        [SerializeField] private bool _autoRotate = true;

        [Header("경계 콜라이더")] [SerializeField] private Collider2D _leftBoundary;
        [SerializeField] private Collider2D _rightBoundary;
        [SerializeField] private Collider2D _topBoundary;
        [SerializeField] private Collider2D _bottomBoundary;

        private Vector2 _direction;
        private float _spawnTime;
        private bool _isActive;
        private bool _isReflect;

        private void Update()
        {
            if (!_isActive) return;

            MoveBullet();
            CheckLifeTime();
        }

        public void Fire(Vector2 direction, float speed = 0f)
        {
            _direction = direction.normalized;

            if (speed > 0f)
            {
                _speed = speed;
            }

            UpdateBulletRotation();

            _spawnTime = Time.time;
            _isActive = true;
            gameObject.SetActive(true);
        }

        private void MoveBullet()
        {
            transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
        }

        private void CheckLifeTime()
        {
            if (Time.time - _spawnTime >= _lifeTime)
            {
                Debug.Log("탄환 수명 만료로 제거됨");
                DestroyBullet();
            }
        }

        private void UpdateBulletRotation()
        {
            if (!_autoRotate) return;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsBoundaryCollider(other))
            {
                HandleBoundaryTeleport(other);
                return;
            }

            HandleTargetCollision(other);
        }

        private void HandleTargetCollision(Collider2D other)
        {
            if (_isReflect)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (other.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.ApplyDamage(1);
                        Debug.Log("적에게 명중!");
                        DestroyBullet();
                    }
                }
            }
            else
            {
                if (other.CompareTag("Player"))
                {
                    if (other.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.ApplyDamage(1);
                        Debug.Log("플레이어에게 명중!");
                        DestroyBullet();
                    }
                }
            }
        }

        private bool IsBoundaryCollider(Collider2D collider)
        {
            return collider == _leftBoundary ||
                   collider == _rightBoundary ||
                   collider == _topBoundary ||
                   collider == _bottomBoundary;
        }

        private void HandleBoundaryTeleport(Collider2D boundaryCollider)
        {
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = currentPosition;
            bool teleportOccurred = false;

            if (boundaryCollider == _leftBoundary && _rightBoundary != null)
            {
                newPosition.x = _rightBoundary.bounds.max.x - 0.1f;
                teleportOccurred = true;
                Debug.Log("탄환이 왼쪽 경계에서 오른쪽으로 순간이동");
            }
            else if (boundaryCollider == _rightBoundary && _leftBoundary != null)
            {
                newPosition.x = _leftBoundary.bounds.min.x + 0.1f;
                teleportOccurred = true;
                Debug.Log("탄환이 오른쪽 경계에서 왼쪽으로 순간이동");
            }
            else if (boundaryCollider == _topBoundary && _bottomBoundary != null)
            {
                newPosition.y = _bottomBoundary.bounds.min.y + 0.1f;
                teleportOccurred = true;
                Debug.Log("탄환이 위쪽 경계에서 아래쪽으로 순간이동");
            }
            else if (boundaryCollider == _bottomBoundary && _topBoundary != null)
            {
                newPosition.y = _topBoundary.bounds.max.y - 0.1f;
                teleportOccurred = true;
                Debug.Log("탄환이 아래쪽 경계에서 위쪽으로 순간이동");
            }

            if (teleportOccurred)
            {
                transform.position = newPosition;
            }
            else
            {
                Debug.LogWarning($"경계 순간이동 실패: {boundaryCollider.name}에 대응하는 반대편 경계를 찾을 수 없음");
            }
        }

        public void DestroyBullet()
        {
            _isReflect = false;
            _isActive = false;
            gameObject.SetActive(false);

            if (BulletPool.Instance != null)
            {
                BulletPool.Instance.ReturnBullet(this);
            }
            else
            {
                Debug.LogWarning("BulletPool 인스턴스를 찾을 수 없어 탄환을 풀로 반환할 수 없음");
            }
        }

        public void SetReflect(bool isReflect)
        {
            _isReflect = isReflect;
            Debug.Log($"탄환 반사 상태 변경: {(_isReflect ? "적 공격 모드" : "플레이어 공격 모드")}");
        }

        public void SetAutoRotate(bool autoRotate)
        {
            _autoRotate = autoRotate;

            if (_autoRotate && _isActive)
            {
                UpdateBulletRotation();
            }
        }

        public Vector2 GetDirection()
        {
            return _direction;
        }

        public bool IsActive()
        {
            return _isActive;
        }

        public bool IsReflected()
        {
            return _isReflect;
        }
    }
}                                       