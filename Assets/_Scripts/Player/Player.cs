using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        private void Awake() { }

        private void Start()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            ParentThisRpc();
        }

        [Rpc(SendTo.Server)]
        private void ParentThisRpc()
        {
            transform.parent = GameObject.Find("PlayerParent").transform;
            LobbyManager.Instance.CheckForPlayersRpc();
        }

        public void OnLeaving()
        {
            Destroy(gameObject);
        }
    }
}
