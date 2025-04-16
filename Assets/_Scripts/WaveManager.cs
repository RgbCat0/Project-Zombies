using System;
using Unity.Netcode;
using UnityEngine;
using _Scripts.Zombies;

namespace _Scripts
{
    // increases enemy amount and difficulty when a wave is completed
    public class WaveManager : NetworkBehaviour
    {
        public static WaveManager Instance { get; private set; }
        private int _currentWave;
        private int _currentEnemyCount;
        private int _currentWaveEnemyCount = 10; // initial enemy count

        [SerializeField]
        private float healthMultiplier = 1f;

        [SerializeField]
        private int extraEnemiesPerWave = 2;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if (!IsServer)
                return;
            _currentWave = 1;
            _currentEnemyCount = 0;
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            for (int i = 0; i < _currentWaveEnemyCount; i++)
            {
                ZombieManager.Instance.SpawnZombie(healthMultiplier);
                _currentEnemyCount++;
            }
            UpdateUIRpc(_currentWave, _currentEnemyCount);
        }

        [Rpc(SendTo.Everyone)]
        private void UpdateUIRpc(int wave, int currEnemies)
        {
            UIManager.Instance.UpdateWave(wave);
            UIManager.Instance.UpdateEnemies(currEnemies);
        }

        [Rpc(SendTo.Server)]
        public void EnemyDiedRpc()
        {
            _currentEnemyCount--;
            if (_currentEnemyCount <= 0)
            {
                OnWaveComplete();
            }
            UIManager.Instance.UpdateEnemies(_currentEnemyCount);
        }

        private void OnWaveComplete()
        {
            if (_currentEnemyCount <= 0)
            {
                _currentWave++;
                healthMultiplier += 0.1f;
                _currentWaveEnemyCount += extraEnemiesPerWave;
                SpawnEnemies();
            }
        }
    }
}
