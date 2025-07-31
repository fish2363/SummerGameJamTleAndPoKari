using Member.CUH.Code.Entities;
using UnityEngine;

namespace Member.ISC.Code.Players
{
    public class PlayerMoveCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Rigidbody2D rb; 
        
        [Header("이동속도 및 회전속도")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeed;
        
        private Player _player;

        private Vector2 _moveDir;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        public void SetDirection(Vector2 dir)
        {
            _moveDir = dir;
        }
    }
}