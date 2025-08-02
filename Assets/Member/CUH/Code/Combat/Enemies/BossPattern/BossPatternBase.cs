using Ami.BroAudio;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.BossPattern
{
    public abstract class BossPatternBase : MonoBehaviour
    {
        public string AnimationString;
        [SerializeField] protected SoundID[] fireSounds;
        protected SoundID sound;
        [SerializeField] protected int fireCount = 3; // 몇 번 발사할거임?
        [SerializeField] protected float fireDelay = 0.2f; // 발사간 사이에 얼마나 기다릴거임?

        protected IDamageable _target;

        public virtual void UsePattern()
        {
            sound = fireSounds[Random.Range(0, fireSounds.Length)];
        }

        public void SetTarget(IDamageable target)
        {
            _target = target;
        }
    }
}