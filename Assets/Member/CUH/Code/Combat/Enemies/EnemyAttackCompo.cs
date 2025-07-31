using DG.Tweening;
using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using Member.ISC.Code.Players;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies
{
    public class EnemyAttackCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private float attackRange = 100f;
        [SerializeField] private float attackCooldown = 1f;
        
        protected Enemy _enemy;
        protected IDamageable _target;
        protected float _lastAtkTime;

        private Sequence shootSequence;

        public virtual void Initialize(Entity entity)
        {
            _enemy = entity as Enemy;
            SetShootSequence();
        }
        
        public void AfterInitialize()
        {
            _target = _enemy.Target;
        }
        
        public virtual bool CanAttack()
        {
            return _lastAtkTime + attackCooldown < Time.time && 
                   Vector2.Distance(_target.transform.position, transform.position) <= attackRange;
        }
        
        public virtual void Attack()
        {
            Debug.Log("저놈추");
            _enemy.DOKill();

            if (shootSequence.IsActive()) DOVirtual.DelayedCall(1f,()=> shootSequence.Restart()); // 매번 새로 만들지 않고 재시작

            _lastAtkTime = Time.time;
        }

        private void SetShootSequence()
        {
            shootSequence = DOTween.Sequence();
            shootSequence.Append(_enemy.transform.DOScale(new Vector3(1.1f, 0.8f, 1f), 0.05f));
            shootSequence.Append(_enemy.transform.DOScale(new Vector3(0.95f, 1.05f, 1f), 0.05f));
            shootSequence.Append(_enemy.transform.DOScale(Vector3.one, 0.02f));
            shootSequence.Pause();                // 바로 실행되지 않게 중지
            shootSequence.SetAutoKill(false);     // 자동 제거 X → 재사용 가능
        }

    }
}