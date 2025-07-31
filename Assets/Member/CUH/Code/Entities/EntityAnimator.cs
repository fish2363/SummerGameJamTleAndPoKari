using System;
using UnityEngine;

namespace Member.CUH.Code.Entities
{
    public class EntityAnimator : MonoBehaviour, IEntityComponent
    {
        public event Action OnAnimationEnd;
        protected Entity _entity;
    
        protected void AnimationEnd()
        {
            OnAnimationEnd?.Invoke();
        }

        public void Initialize(Entity entity)
        {
            _entity = entity;    
        }
    }
}
