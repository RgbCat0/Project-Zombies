using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        public void OnCreated(ulong playerId)
        {
            // todo: change owner, set player name, etc.
            NetworkObject.Spawn();
            NetworkObject.ChangeOwnership(playerId);
        }

        public void OnLeaving()
        {
            Destroy(gameObject);
        }
    }
}
