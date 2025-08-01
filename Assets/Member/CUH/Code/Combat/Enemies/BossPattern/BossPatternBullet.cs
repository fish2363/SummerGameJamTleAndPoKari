using Member.KDH.Code.Bullet;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public abstract class BossPatternBullet : BossPatternBase
    {
        [SerializeField] protected Bullet bulletPrefab;
        [SerializeField] protected float bulletSpeed = 3f;
        [SerializeField] protected float bulletLifeTime = 5f;
    }
}