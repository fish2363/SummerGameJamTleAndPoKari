using System.Collections;
using Ami.BroAudio;
using Member.KDH.Code.Bullet;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public class BossPatternLaserAndBullet : BossPatternBase
    {
        [SerializeField] private Laser laserPrefab;
        [SerializeField] private float laserScale = 10f;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletSpeed = 3f;
        [SerializeField] private float bulletLifeTime = 5f;
        [SerializeField] private float spreadAngle = 15f; // 좌우 퍼짐 각도
        [SerializeField] private float waitTime = 0.5f; // 레이저와 총알 사이의 시간
        
        private Vector3[] _targetPositions = {Vector3.left, Vector3.down, Vector3.right, Vector3.up};
        
        public override void UsePattern()
        {
            base.UsePattern();
            StartCoroutine(LaserAndBulletAttack());
        }

        private IEnumerator LaserAndBulletAttack()
        {
            for (int i = 0; i < fireCount; i++)
            {
                Laser laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
                laser.transform.localScale *= laserScale;
                Vector3 targetPos = _targetPositions[Random.Range(0, _targetPositions.Length)];
                laser.Shoot(targetPos, transform, laserScale);
                sound.Play();
                yield return new WaitForSeconds(waitTime);
                FireBullet(targetPos);
                Vector2 leftDir = RotateVector(targetPos, spreadAngle);
                Vector2 rightDir = RotateVector(targetPos, -spreadAngle);

                FireBullet(leftDir);
                FireBullet(rightDir);
                yield return new WaitForSeconds(fireDelay);
            }
        }
        
        private void FireBullet(Vector2 direction)
        {
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.Fire(direction.normalized, bulletSpeed);
            Destroy(bullet.gameObject, bulletLifeTime);
        }

        private Vector2 RotateVector(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(
                v.x * cos - v.y * sin,
                v.x * sin + v.y * cos
            ).normalized;
        }
    }
}