using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "ZombieInfo", menuName = "ScriptableObjects/ZombieInfo", order = 1)]
    public class ZombieInfo : ScriptableObject
    {
        public string zombieName;
        public NetworkObject zombiePrefab;
        public GameObject model; // model to spawn for different zombies
        public float healthMultiplier;
        public float speedMultiplier;
        public float attackDamage;
    }
}
