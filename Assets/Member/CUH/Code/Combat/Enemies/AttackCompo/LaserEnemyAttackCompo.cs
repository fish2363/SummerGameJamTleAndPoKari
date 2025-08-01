using System;
using DG.Tweening;
using UnityEngine;

namespace Member.CUH.Code.Combat.Enemies.AttackCompo
{
    public class LaserEnemyAttackCompo : EnemyAttackCompo
    {
        [SerializeField] private Laser laserPrefab;
        
        public override void Attack()
        {
            base.Attack();
            Laser laser = Instantiate(laserPrefab, _enemy.transform.position, Quaternion.identity);
            laser.Shoot(_target.transform.position, _enemy.transform);
        }
    }
}