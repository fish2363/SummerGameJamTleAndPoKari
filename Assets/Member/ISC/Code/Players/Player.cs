using Member.CUH.Code.Entities;
using Member.ISC.Code.Managers;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class Player : Entity
    {
        [field: SerializeField] public InputManagerSO PlayerInput { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            
        }
    }
}