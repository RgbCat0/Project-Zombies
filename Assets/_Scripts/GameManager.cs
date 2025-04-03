using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;

        }

        public void LoadInPlayers()
        {
            // should start in the main game scene
            if (IsServer)
            {
                Debug.Log("Loading in players");
                NetworkManager.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
            }

        }

        private void SceneManagerOnOnLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode)
        {
            Debug.Log("Loading in players!");
            if(scenename != "Main")
                return;

            var playerId = LobbyManager.Instance.ConvertedIds[clientid];
            var playerName = LobbyUtil.GetPlayerNameById(playerId);
            Debug.Log($"Player {playerName} joined.");
        }


    }
}
