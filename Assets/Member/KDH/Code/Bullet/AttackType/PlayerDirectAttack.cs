using Ami.BroAudio;
using Member.CUH.Code.Entities;
using Member.KDH.Code.Bullet;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies
{
    public class PlayerDirectAttack : EnemyAttackCompo
    {
        [Header("플레이어 방향 공격 설정")]
        [SerializeField] private float _bulletSpeed = 1f;   // 탄환 속도
        [SerializeField] private SoundID[] enemyAttackSounds;
        
        private Transform _playerTransform;
        private float _lastAttackTime;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            FindPlayer();
        }
        
        private void Update()
        {
            if (_playerTransform == null)
            {
                FindPlayer();
                return;
            }
            
            // if (Time.time - _lastAttackTime >= _attackInterval)
            // {
            //     Attack();
            // }
        }
        
        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("플레이어를 찾을 수 없습니다!");
            }
        }

        public override void Attack()
        {
            base.Attack();
            
            if (_playerTransform == null || BulletPool.Instance == null) return;
            
            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            
            Bullet bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = transform.position;
            int i = Random.Range(0, enemyAttackSounds.Length);
            enemyAttackSounds[i].Play();
            bullet.Fire(direction, _bulletSpeed);
            
            _lastAttackTime = Time.time;
        }
    }
}