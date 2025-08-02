using System.Collections;
using Ami.BroAudio;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public class BosssPatternCircularBullet : BossPatternBullet
    {
        [SerializeField] private int bulletsPerShot = 8;
        [SerializeField] private float angleOffsetPerFire = 45f; // 발사마다 도는 각도

        public override void UsePattern()
        {
            base.UsePattern();
            StartCoroutine(FireRoutine());
        }

        private IEnumerator FireRoutine()
        {
            float angleOffset = 0f;

            for (int i = 0; i < fireCount; i++)
            {
                FireCircle(angleOffset);
                sound.Play();
                angleOffset += angleOffsetPerFire;
                yield return new WaitForSeconds(fireDelay);
            }
        }

        private void FireCircle(float angleOffset)
        {
            float angleStep = 360f / bulletsPerShot;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                float angle = angleOffset + (angleStep * i);
                Vector2 dir = AngleToDirection(angle);
                FireBullet(dir);
            }
        }

        private void FireBullet(Vector2 direction)
        {
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.Fire(direction.normalized, bulletSpeed);
            Destroy(bullet.gameObject, bulletLifeTime);
        }

        private Vector2 AngleToDirection(float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
    }
}