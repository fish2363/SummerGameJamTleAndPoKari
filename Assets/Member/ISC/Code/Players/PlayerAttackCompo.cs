using System;
using DG.Tweening;
using Member.CUH.Code.Combat;
using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using Member.KDH.Code.Bullet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Member.ISC.Code.Players
{
    public class PlayerAttackCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private LayerMask whatIsTarget;
        
        [SerializeField] private float castRadius;
        [SerializeField] private float parryRadius;
        [SerializeField] private float damage;

        [SerializeField] private ParticleSystem breakParticle;
        [SerializeField] private ParticleSystem parryParticle;
        
        private bool isParry = false;
        private EntityAnimatorTrigger _triggers;
        private Player _player;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            originalPos = Camera.main.transform.localPosition;
        }

        public void Attack()
        {
            Collider2D[] c = Physics2D.OverlapCircleAll(_player.transform.position, castRadius, whatIsTarget);

            if (c.Length > 0)
            {
                foreach (Collider2D item in c)
                {
                    if (InSight(item.transform.position))
                    {
                        float distance = Vector2.Distance(_player.transform.position, item.transform.position);
                        if (distance > (castRadius - parryRadius))
                        {
                            isParry = true;
                        }
                        Bullet b = item.gameObject.GetComponent<Bullet>();
                            
                        if (item.TryGetComponent(out IDamageable d))
                        {
                            ShakeCamera();
                            d.ApplyDamage(damage);
                        }
                        else if (isParry)
                        {
                            b.SetReflect(true);
                            parryParticle.Play();
                            Instantiate(parryParticle, b.transform);
                            b.Fire(_player.transform.right);
                        }
                        else
                        {
                            Instantiate(breakParticle, transform.position, Quaternion.identity).Play();
                            b?.DestroyBullet();
                        }
                    }
                }
            }
            
            isParry = false;
        }

        private bool InSight(Vector3 target)
        {
            Vector3 dir =  target - _player.transform.position;

            float value = Vector3.Dot(_player.transform.right, dir.normalized);
            return value > 0;
        }
        public float shakeDuration = 0.3f;
        public float shakeStrength = 0.3f;

        private Vector3 originalPos;
        public void ShakeCamera()
        {
            Camera.main.transform.localPosition = originalPos; // 혹시 이전 흔들림에서 안 돌아왔으면 리셋

            Camera.main.transform
                .DOShakePosition(shakeDuration, shakeStrength, vibrato: 10, randomness: 90, snapping: false, fadeOut: true)
                .OnComplete(() => Camera.main.transform.localPosition = originalPos); // 흔들고 난 뒤 원위치 보정
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, castRadius);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, castRadius - parryRadius);
        }

        
#endif
    }
}