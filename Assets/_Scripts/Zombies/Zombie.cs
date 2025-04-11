using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class Zombie : NetworkBehaviour // handles health and general
    {
        [SerializeField]
        private NetworkVariable<int> health = new(100);

        [SerializeField]
        private int pointAmount = 10;

        [Rpc(SendTo.Server)]
        public void TakeDamageRpc(int damage)
        {
            health.Value -= damage;
            if (health.Value <= 0)
                DieRpc();
        }

        [Rpc(SendTo.Server)]
        private void DieRpc()
        {
            DieAllRpc();
            // handle death
            // play animation
            // destroy object
            PointManager.Instance.AddPoints(pointAmount);
            NetworkObject.Despawn();
        }

        [Rpc(SendTo.Everyone)]
        private void DieAllRpc()
        {
            // man.
        }
    }
}
