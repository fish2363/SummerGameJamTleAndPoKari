using System;
using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class PlayerAttackCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private float castRadius;
        [SerializeField] private float parryRadius;

        private bool isParry;

        private Player _player;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
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