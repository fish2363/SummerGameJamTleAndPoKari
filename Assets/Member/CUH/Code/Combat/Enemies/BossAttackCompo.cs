using Member.CUH.Code.Combat.Enemies.BossPattern;
using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies
{
    public class BossAttackCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private BossPatternBase[] bossPatterns;
        
        [SerializeField] private float attackCooldown = 5f;
        
        protected Boss _boss;
        protected IDamageable _target;
        protected float _lastAtkTime;
        
        public void Initialize(Entity entity)
        {
            _boss = entity as Boss;
            bossPatterns = GetComponentsInChildren<BossPatternBase>();
        }

        public void AfterInitialize()
        {
            _target = _boss.Target;
            _lastAtkTime = Time.time;
            foreach (var pattern in bossPatterns)
            {
                pattern.SetTarget(_target);
            }
        }

        public virtual bool CanAttack()
            => _lastAtkTime + attackCooldown < Time.time;
        
        public virtual void Attack()
        {
            _lastAtkTime = Time.time;
            bossPatterns[Random.Range(0,bossPatterns.Length)].UsePattern();
        }
    }
}