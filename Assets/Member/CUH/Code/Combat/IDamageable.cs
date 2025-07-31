using UnityEngine;

namespace Member.CUH.Code.Combat
{
    public interface IDamageable
    {
        public Transform transform { get; }
        public void ApplyDamage(float damage);
    }
}