using System.Collections;
using Ami.BroAudio;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public class BossPatternLaserRampage : BossPatternLaser
    {
        [SerializeField] private float randomValue = 1f;
        
        public override void UsePattern()
        {
            base.UsePattern();
            StartCoroutine(LaserRampage());
        }

        private IEnumerator LaserRampage()
        {
            for (int i = 0; i < fireCount; i++)
            {
                Laser laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
                laser.Shoot(_target.transform.position + (Vector3)Random.insideUnitCircle * randomValue, transform);
                sound.Play();
                yield return new WaitForSeconds(fireDelay);
            }
        }
    }
}