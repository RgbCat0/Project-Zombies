using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Zombies;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts
{
    // increases enemy amount and difficulty when a wave is completed
    public class WaveManager : NetworkBehaviour
    {
        public static WaveManager Instance { get; private set; }
        private int _currentWave;
        private int _currentEnemyCount;
        private int _currentWaveEnemyCount = 10; // initial enemy count
        public List<Wave> waves = new(); // holds info for what zombies to spawn and from when to start and end
        public List<ZombieInfo> zombies = new(); // cache so it doesnt need to send it over RPC every time

        [SerializeField]
        private int extraEnemiesPerWave = 2;

        [SerializeField]
        public float spawnDelay = 0.9f;
        public float healthMulti = 0f; // multi added per wave on top of the default health

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
            UpdateUIRpc(_currentWave, _currentWaveEnemyCount);
            // get the current wave
            Wave waveToSpawn = waves.First(wave => wave.startWave >= _currentWave);
            StartCoroutine(ZombieManager.Instance.SpawnZombie(waveToSpawn, _currentWaveEnemyCount));
            _currentEnemyCount += _currentWaveEnemyCount;
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
                StartNextWave();
            }
            UIManager.Instance.UpdateEnemies(_currentEnemyCount);
        }

        public void StartNextWave()
        {
            _currentWave++;
            healthMulti += 0.1f;
            _currentWaveEnemyCount += extraEnemiesPerWave;
            GameManager.Instance.RespawnPlayersRpc();
            SpawnEnemies();
        }

        public void DespawnEnemies()
        {
            foreach (var zombie in ZombieManager.Instance.zombies)
            {
                if (zombie != null)
                {
                    zombie.NetworkObject.Despawn();
                }
            }
        }
    }

    [Serializable]
    // does not hold info for how many zombies to spawn, only what types of zombies and their spawn chances
    public class Wave // holds info for what zombies to spawn and from when to start and end
    {
        public int startWave;
        public List<ZombieSpawnInfo> zombies;

        public ZombieInfo GetRandomZombieInfo()
        {
            float totalChance = zombies.Sum(zombieInfo => zombieInfo.spawnChance);

            float randomValue = UnityEngine.Random.Range(0, totalChance); // random value between 0 and total chance
            float cumulativeChance = 0; // cumulative chance of the zombies

            foreach (var zombieInfo in zombies)
            { // for example zombie 1 has a spawn chance of 0.5 and zombie 2 has a spawn chance of 0.5, the random value will be between 0 and 1
                cumulativeChance += zombieInfo.spawnChance; // add the chance of the current zombie
                if (randomValue <= cumulativeChance)
                {
                    return zombieInfo.info;
                }
            }

            return null; // This should never happen if the spawn chances are set up correctly
        }
    }

    [Serializable]
    public class ZombieSpawnInfo
    {
        public ZombieInfo info;
        public float spawnChance; // chance of the zombie to spawn
    }
}
