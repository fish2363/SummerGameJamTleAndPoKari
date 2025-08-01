using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public abstract class BossPatternBase : MonoBehaviour
    {
        [SerializeField] protected int fireCount = 3; // 몇 번 발사할거임?
        [SerializeField] protected float fireDelay = 0.2f; // 발사간 사이에 얼마나 기다릴거임?

        protected IDamageable _target;
        
        public abstract void UsePattern();

        public void SetTarget(IDamageable target)
        {
            _target = target;
        }
    }
}