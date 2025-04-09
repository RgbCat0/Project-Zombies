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
            if (!IsOwnedByServer)
                enabled = false;
            SpawnZombie(); // testing
        }

        public void SpawnZombie(Vector3 position = default)
        {
            // Spawn a zombie at the given position
            if (position == default)
            {
                position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            }

            var zombie = NetworkManager.SpawnManager.InstantiateAndSpawn(
                zombiePrefab,
                position: position
            );
            var zombieComponent = zombie.GetComponent<Zombie>();
            if (zombieComponent != null)
            {
                zombieComponent.transform.position = position;
                zombies.Add(zombieComponent);
            }
            else
            {
                Debug.LogError("Zombie component not found on the spawned prefab.");
            }
        }
    }
}
