using Chuh007Lib.Dependencies;
using Member.ISC.Code.Players;
using UnityEngine;

namespace Member.CUH.Code.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy[] spawnEnemies;
        
        [Inject] private Player _player;
    }
}