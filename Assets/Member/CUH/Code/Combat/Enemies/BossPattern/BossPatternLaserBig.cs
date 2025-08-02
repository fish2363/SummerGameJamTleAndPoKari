using System.Collections;
using Ami.BroAudio;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public class BossPatternLaserBig : BossPatternLaser
    {
        [SerializeField] private float laserScale = 10f;
        
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
                laser.transform.localScale *= laserScale;
                laser.Shoot(_target.transform.position, transform, laserScale);
                sound.Play();
                yield return new WaitForSeconds(fireDelay);
            }
        }
    }
}