using UnityEngine;

namespace Member.CUH.Code.Entities
{
    public class EntityRenderer : MonoBehaviour, IEntityComponent
    {
        private Entity _entity;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        public void RotateToTarget(Transform target)
        {
            Vector2 direction = target.position - _entity.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        public void Rotate(float angle)
        {
            transform.Rotate(new Vector3(0f, 0f, angle));
        }
    }
}
