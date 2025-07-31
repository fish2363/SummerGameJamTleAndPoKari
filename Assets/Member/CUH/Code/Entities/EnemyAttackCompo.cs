using UnityEngine;

namespace Member.CUH.Code.Entities
{
    public abstract class EnemyAttackCompo : MonoBehaviour, IEntityComponent
    {
        protected Entity _entity;
        public virtual void Initialize(Entity entity)
        {
            _entity = entity;
        }
        
    }
}