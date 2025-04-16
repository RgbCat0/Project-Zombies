using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class ZombieManager : NetworkBehaviour
    {
        public static ZombieManager Instance { get; private set; }

        [SerializeField]
        public List<Zombie> zombies;

        [SerializeField]
        private NetworkObject zombiePrefab;

        [SerializeField]
        private List<Transform> zombieSpawnPoints;

        [SerializeField]
        private Transform zombieParent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Initialize the zombie manager
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public IEnumerator SpawnZombie(Wave waveToSpawn, int enemiesCount)
        {
            if (!NetworkManager.Singleton.IsServer)
                yield break;
            for (int i = 0; i < enemiesCount; i++)
            {
                // pick a random spawn point
                int randomIndex = Random.Range(0, zombieSpawnPoints.Count);
                Transform spawnPoint = zombieSpawnPoints[randomIndex];
                // pick a random zombieInfo based on the zombieInfo spawn chance
                ZombieInfo zombieInfo = waveToSpawn.GetRandomZombieInfo();
                // check if the zombieInfo is null
                if (zombieInfo == null)
                {
                    Debug.LogError("ZombieInfo is null");
                    continue;
                }
                // spawn the zombie
                yield return new WaitForSeconds(WaveManager.Instance.spawnDelay);
                SpawnZombieAtPoint(zombieInfo, spawnPoint);
            }

            // old code
            // Debug.Log(spawnPoint.name);
            // NetworkObject zombie = NetworkManager.SpawnManager.InstantiateAndSpawn(zombiePrefab);
            // zombie.transform.parent = zombieParent.transform;
            // var zombieComponent = zombie.GetComponent<Zombie>();
            // if (zombieComponent != null)
            // {
            //     zombieComponent.SetupRpc(healthMulti, spawnPoint.position);
            //     zombieComponent.transform.position = spawnPoint.position;
            //     zombies.Add(zombieComponent);
            // }
            // else
            // {
            //     Debug.LogError("Zombie component not found on the spawned prefab.");
            // }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnZombieAtPoint(ZombieInfo zombieInfo, Transform spawnPoint)
        {
            NetworkObject zombie = NetworkManager.SpawnManager.InstantiateAndSpawn(
                zombieInfo.zombiePrefab
            );
            zombie.transform.parent = zombieParent;
            var zombieComponent = zombie.GetComponent<Zombie>();
            zombieComponent.SetupRpc(
                // zombieInfo.healthMultiplier + WaveManager.Instance.healthMulti,
                // zombieInfo.speedMultiplier,
                // zombieInfo.model
                zombieInfo.zombieName,
                spawnPoint.position
            );
            zombies.Add(zombieComponent);
        }
    }
}
