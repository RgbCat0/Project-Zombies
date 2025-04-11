using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class ZombieManager : NetworkBehaviour
    {
        public static ZombieManager Instance { get; private set; }

        [SerializeField]
        private List<Zombie> zombies;

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
                DontDestroyOnLoad(gameObject);
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
                return;
            }
            ;
            for (int i = 0; i < 20; i++)
            {
                SpawnZombie(); // testingx
            }
        }

        public void SpawnZombie(Transform spawnPoint = null)
        {
            if (spawnPoint is null)
            {
                var randomIndex = Random.Range(0, zombieSpawnPoints.Count);
                spawnPoint = zombieSpawnPoints[randomIndex];
            }

            var zombie = NetworkManager.SpawnManager.InstantiateAndSpawn(
                zombiePrefab,
                position: spawnPoint.position
            );
            zombie.transform.parent = zombieParent.transform;
            var zombieComponent = zombie.GetComponent<Zombie>();
            if (zombieComponent != null)
            {
                zombieComponent.transform.position = spawnPoint.position;
                zombies.Add(zombieComponent);
            }
            else
            {
                Debug.LogError("Zombie component not found on the spawned prefab.");
            }
        }
    }
}
