using System;
using System.Collections;
using Chuh007Lib.Dependencies;
using Member.CUH.Code.Combat;
using Member.ISC.Code.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Member.CUH.Code.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject warningObject;
        [SerializeField] private Enemy[] spawnEnemies;
        [SerializeField] private Transform leftBottomTrm;
        [SerializeField] private Transform rightTopTrm;
        
        [Inject] private Player _player;

        [SerializeField] private int _spawnEnemyCount = 2;
        [SerializeField] private float _spawnCooldown = 15f;
        private int _currentEnemyCount = 0;
        private int nextEnemyCountUpScore = 5;
        private float _lastSpawnTime;
        
        private void Start()
        {
            SpawnEnemies();
        }

        private void Update()
        {
            if (_lastSpawnTime + _spawnCooldown <= Time.time)
            {
                SpawnEnemies();
            }
        }

        private void SpawnEnemies()
        {
            _lastSpawnTime = Time.time;
            for (int i = 0; i < _spawnEnemyCount; i++)
            {
                StartCoroutine(SpawnEnemy());
            }
        }
        
        private IEnumerator SpawnEnemy()
        {
            float x = Random.Range(leftBottomTrm.position.x, rightTopTrm.position.x);
            float y = Random.Range(leftBottomTrm.position.y, rightTopTrm.position.y);
            Vector2 spawnPos = new Vector2(x, y);
            GameObject obj = Instantiate(warningObject, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(1f);
            Destroy(obj);
            Enemy spawnEnemy = Instantiate(spawnEnemies[Random.Range(0, spawnEnemies.Length)], spawnPos, Quaternion.identity);
            spawnEnemy.SetTarget(_player.GetComponent<IDamageable>());
            _currentEnemyCount++;
            spawnEnemy.OnDeadEvent.AddListener(HandleEnemyDead);
        }

        private void HandleEnemyDead()
        {
            _currentEnemyCount--;
            ScoreManager.Instance.Score(1);
            if (ScoreManager.Instance.CurrentScore >= nextEnemyCountUpScore)
            {
                nextEnemyCountUpScore += nextEnemyCountUpScore / 2;
                _spawnEnemyCount++;
            }

            if (_currentEnemyCount <= 0)
            {
                SpawnEnemies();
            }
        }
    }
}