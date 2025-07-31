using System;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class PlayerAttackCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private LayerMask whatIsTarget;
        
        [SerializeField] private float castRadius;
        [SerializeField] private float parryRadius;

        private bool isParry;

        private Player _player;

        private Collider2D[] targetArr;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        [ContextMenu("테스트용 어택")]
        public void Attack()
        {
            int c = Physics2D.OverlapCircleNonAlloc(_player.transform.position, castRadius, targetArr, whatIsTarget);

            if (c > 0)
            {
                foreach (Collider2D item in targetArr)
                {
                    if (InSight(item.transform.position))
                    {
                        Debug.Log("앞!");
                    }
                }
            }
        }

        private bool InSight(Vector3 target)
        {
            Vector3 dir = _player.transform.position - target;

            float value = Vector3.Dot(dir, target);

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