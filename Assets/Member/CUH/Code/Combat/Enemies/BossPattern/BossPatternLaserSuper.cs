using System.Collections;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public class BossPatternLaserSuper : BossPatternLaser
    {
        [SerializeField] private float rotationValue = 5f;
        
        public override void UsePattern()
        {
            StartCoroutine(FireLaserRoutine());
        }
        
        private IEnumerator FireLaserRoutine()
        {
            for (int i = 0; i < fireCount; i++)
            {
                float rad = rotationValue * i * Mathf.Deg2Rad;
                Laser laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
                Vector2 toPos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                laser.Shoot(toPos.normalized, transform);
                yield return new WaitForSeconds(fireDelay);
            }
        }
    }
}