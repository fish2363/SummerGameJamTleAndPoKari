using System;
using System.Collections;
using Chuh007Lib.Dependencies;
using Member.CUH.Code.Combat;
using Member.ISC.Code.Players;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Random = UnityEngine.Random;

namespace Member.CUH.Code.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance;

        public UnityEvent OnBossStartEvent; 
        
        [field: SerializeField] public int OverClockEnemyCount { get; private set; }
        [SerializeField] private int bossCutWave;
        
        [SerializeField] private GameObject warningObject;
        [SerializeField] private Enemy[] spawnEnemies;
        [SerializeField] private Boss[] spawnBosses;
        [SerializeField] private Transform leftBottomTrm;
        [SerializeField] private Transform rightTopTrm;

        [Header("생성 워닝 띄우는 시간")]
        public float totalDuration = 5f;
        [Header("처음 깜빡임 간격")]
        public float startInterval = 1.0f;
        [Header("마지막 깜빡임 간격")]
        public float endInterval = 0.1f;

        [Inject] private Player _player;

        [SerializeField] private int _spawnEnemyCount = 2;
        [SerializeField]private int _currentEnemyCount = 0;
        private int nextEnemyCountUpScore = 5;
        private int _currentWave = 1;

        private void Awake()
        {
            Instance = this;

            OnBossStartEvent.AddListener(HandleBossStartEvent);
        }

        private void HandleBossStartEvent()
        {
            StartCoroutine(SpawnBoss());
        }

        private IEnumerator SpawnBoss()
        {
            Debug.Log("보스가 왔다");
            GameObject obj = Instantiate(warningObject, Vector2.zero, Quaternion.identity);
            float elapsed = 0f;
            bool visible = true;
            while (elapsed < totalDuration)
            {
                float t = elapsed / totalDuration;
                float currentInterval = Mathf.Lerp(startInterval, endInterval, t);

                obj.SetActive(visible);
                visible = !visible;

                yield return new WaitForSeconds(currentInterval);
                elapsed += currentInterval;
            }
            Destroy(obj);
            _currentEnemyCount++;
            Boss spawnBoss = Instantiate(spawnBosses[Random.Range(0, spawnBosses.Length)], Vector2.zero, Quaternion.identity);
            spawnBoss.SetTarget(_player.GetComponent<IDamageable>());
            spawnBoss.OnDeadEvent.AddListener(HandleEnemyDead);
        }
        
        private void Start()
        {
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
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
            float elapsed = 0f;
            bool visible = true;
            while (elapsed < totalDuration)
            {
                float t = elapsed / totalDuration;
                float currentInterval = Mathf.Lerp(startInterval, endInterval, t);

                obj.SetActive(visible);
                visible = !visible;

                yield return new WaitForSeconds(currentInterval);
                elapsed += currentInterval;
            }
            Destroy(obj);
            Enemy spawnEnemy = Instantiate(spawnEnemies[Random.Range(0, spawnEnemies.Length)], spawnPos, Quaternion.identity);
            spawnEnemy.SetTarget(_player.GetComponent<IDamageable>());
            _currentEnemyCount++;
            spawnEnemy.OnDeadEvent.AddListener(HandleEnemyDead);
            spawnEnemy.OnOverClock += HandleOverClock;
        }

        private void HandleOverClock(bool isOn)
        {
            OverClockEnemyCount += isOn ? 1 : -1;
        }

        [ContextMenu("웨이브 체크")]
        public void HandleEnemyDead()
        {
            _currentEnemyCount--;
            if (ScoreManager.Instance.CurrentScore >= nextEnemyCountUpScore)
            {
                nextEnemyCountUpScore += (int)(nextEnemyCountUpScore * 1.5f);
                _spawnEnemyCount++;
            }

            if (_currentEnemyCount <= 0)
            {
                _currentWave++;
                if (bossCutWave != 0 && (_currentWave) % bossCutWave == 0)
                {
                    OnBossStartEvent?.Invoke();
                    return;
                }
                SpawnEnemies();
            }
        }
    }
}