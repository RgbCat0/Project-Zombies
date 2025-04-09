using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class Zombie : NetworkBehaviour // handles health and general
    {
        [SerializeField]
        private NetworkVariable<int> health = new(100);

        private void Start() // server should be owner of this object.
        { }

        [Rpc(SendTo.Server)]
        public void TakeDamageRpc(int damage)
        {
            Debug.Log($"Zombie took {damage} damage.");
            health.Value -= damage;
            if (health.Value <= 0)
                DieRpc();
        }

        [Rpc(SendTo.Server)]
        private void DieRpc()
        {
            Debug.Log("Zombie died.");
            DieAllRpc();
            // handle death
            // play animation
            // destroy object
        }

        [Rpc(SendTo.Everyone)]
        private void DieAllRpc()
        {
            // man.
        }
    }
}
