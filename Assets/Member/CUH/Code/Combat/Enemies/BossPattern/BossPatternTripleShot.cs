using System.Collections;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public class TripleShotPattern : BossPatternBullet
    {
        [SerializeField] private float spreadAngle = 15f; // 좌우 퍼짐 각도

        public override void UsePattern()
        {
            if (_target == null) return;
            StartCoroutine(FireLoop());
        }

        private IEnumerator FireLoop()
        {
            for (int i = 0; i < fireCount; i++)
            {
                FireTriple();
                yield return new WaitForSeconds(fireDelay);
            }
        }

        private void FireTriple()
        {
            Vector2 baseDir = ((Vector2)_target.transform.position - (Vector2)transform.position).normalized;
            FireBullet(baseDir);
            Vector2 leftDir = RotateVector(baseDir, spreadAngle);
            Vector2 rightDir = RotateVector(baseDir, -spreadAngle);

            FireBullet(leftDir);
            FireBullet(rightDir);
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