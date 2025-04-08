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
        }

        public void SpawnZombie(Vector3 position = default)
        {
            // Spawn a zombie at the given position
        }
    }
}
