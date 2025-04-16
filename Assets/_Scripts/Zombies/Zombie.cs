using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class Zombie : NetworkBehaviour // handles health and general
    {
        [SerializeField]
        private NetworkVariable<int> health = new(100);
        private int _defaultHealth = 100;

        [SerializeField]
        private int pointAmount = 10;

        public void Setup(float healthMulti)
        {
            health.Value = (int)(_defaultHealth * healthMulti);
        }

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
            // handle death
            // play animation
            // destroy object
            PointManager.Instance.AddPoints(pointAmount);
            WaveManager.Instance.EnemyDiedRpc();
            GameManager.Instance.RespawnPlayersRpc();
            NetworkObject.Despawn();
        }
    }
}
