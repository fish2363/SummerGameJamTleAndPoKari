using System.Collections.Generic;
using UnityEngine;

namespace Member.KDH.Code.Bullet
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance { get; private set; }
        
        [Header("풀링 설정")]
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private int _poolSize = 100;
        
        private Queue<Bullet> _bulletPool;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializePool();
            }
        }
        
        private void InitializePool()
        {
            _bulletPool = new Queue<Bullet>();
            
            for (int i = 0; i < _poolSize; i++)
            {
                Bullet bullet = Instantiate(_bulletPrefab, transform);
                bullet.gameObject.SetActive(false);
                _bulletPool.Enqueue(bullet);
            }
            
            Debug.Log($"탄환 풀 초기화 완료: {_poolSize}개");
        }
        
        public Bullet GetBullet()
        {
            if (_bulletPool.Count > 0)
            {
                return _bulletPool.Dequeue();
            }
            
            Debug.LogWarning("탄환 풀이 부족합니다. 새 탄환을 생성합니다.");
            return Instantiate(_bulletPrefab, transform);
        }
        
        public void ReturnBullet(Bullet bullet)
        {
            if (bullet != null)
            {
                bullet.transform.SetParent(transform);
                _bulletPool.Enqueue(bullet);
            }
        }
    }
}