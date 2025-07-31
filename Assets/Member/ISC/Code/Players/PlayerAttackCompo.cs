using System;
using Member.CUH.Code.Combat;
using Member.CUH.Code.Enemies;
using Member.CUH.Code.Entities;
using Member.KDH.Code.Bullet;
using UnityEngine;
using UnityEngine.Events;

namespace Member.ISC.Code.Players
{
    public class PlayerAttackCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private LayerMask whatIsTarget;
        
        [SerializeField] private float castRadius;
        [SerializeField] private float parryRadius;
        [SerializeField] private float damage;
        
        private bool isParry = false;

        private Player _player;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
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
                            Debug.Log("패링!");
                            isParry = true;
                        }
                        Bullet b = item.gameObject.GetComponent<Bullet>();
                            
                        if (item.TryGetComponent(out IDamageable d))
                        {
                            d.ApplyDamage(damage);
                        }
                        else if (isParry)
                        {
                            b.SetReflect(true);
                            b.Fire(_player.transform.right);
                        }
                        else
                            b?.DestroyBullet();
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