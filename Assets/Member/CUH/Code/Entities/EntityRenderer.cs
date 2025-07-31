using UnityEngine;

namespace Member.CUH.Code.Entities
{
    public class EntityRenderer : AnimatorCompo, IEntityComponent
    {
        public float FacingDirection { get; private set; } = 1;
    
        private Entity _entity;
    
    
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }
    
        #region FlipController
        public void Flip()
        {
            FacingDirection *= -1;
            _entity.transform.Rotate(0, 180f, 0);
        }
        public void FlipController(float xMove)
        {
            if (Mathf.Abs(FacingDirection + xMove) < 0.5f)
            {
                Flip();
            }
        }
        #endregion
    }
}
