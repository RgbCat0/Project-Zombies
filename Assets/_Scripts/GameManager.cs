using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts
{
    public class GameManager : NetworkBehaviour
    {
        private void Start()
        {
            // should start in the main game scene
            if (IsServer)
            {
                // server logic
            }
            else
            {
                // client logic
            }
        }
    }
}
