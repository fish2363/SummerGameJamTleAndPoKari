using Member.KDH.Code.Bullet;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public abstract class BossPatternBullet : BossPatternBase
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletLifeTime;
    }
}