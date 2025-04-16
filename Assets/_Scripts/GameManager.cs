using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Scripts.LobbyScripts;
using _Scripts.Player;
using _Scripts.Zombies;

namespace _Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        private int _playerAmount;
        private int _playersLoaded;

        public List<Player.Player> players = new();
        public List<PlayerMovement> playerMovements = new(); // Player.Player spawns in this player and is another object
        public List<PlayerMovement> diedPlayers = new();

        [SerializeField]
        private Transform playerSpawnPoint;

        private GameObject _inGameManager;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        [Rpc(SendTo.Everyone)]
        public void LoadInPlayersRpc()
        {
            // should start in the main game scene
            GetAllPlayers();
            if (IsServer)
            {
                // Debug.Log("Loading in players");
                NetworkManager.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
            }
        }

        private void SceneManagerOnOnLoadComplete(
            ulong clientid,
            string scenename,
            LoadSceneMode loadscenemode
        )
        {
            if (scenename != "Main")
                return;
            if (clientid == OwnerClientId)
            {
                // _inGameManager = GameObject.FindWithTag("InGameManager");
                // _inGameManager
                //     .GetComponent<ZombieManager>()
                //     .NetworkObject.ChangeOwnership(OwnerClientId);
            }

            string playerId = LobbyManager.Instance.ConvertedIds[clientid];
            // string playerName = LobbyUtil.GetName(playerId);
            // Debug.Log($"Player {playerName} loaded in."); (debug)
            _playersLoaded++;
            if (_playersLoaded == _playerAmount)
                SpawnInPlayerObjectsRpc();
        }

        private void GetAllPlayers()
        {
            // Debug.Log(NetworkManager.Singleton.ConnectedClients.Count);
            foreach (var player in NetworkManager.Singleton.ConnectedClients)
            {
                var currPlayer = player.Value.PlayerObject.GetComponent<Player.Player>();
                players.Add(currPlayer);
            }
            _playerAmount = players.Count;
        }

        [Rpc(SendTo.Server)]
        private void SpawnInPlayerObjectsRpc()
        {
            foreach (var player in players)
                player.SpawnInThisPlayer();

            AddPlayerMovementsRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void AddPlayerMovementsRpc()
        {
            playerSpawnPoint = GameObject.FindWithTag("SpawnPos").transform;
            foreach (var player in players)
            {
                var playerMovement = player.inGamePlayer.GetComponent<PlayerMovement>();
                playerMovements.Add(playerMovement);
            }
            ZombieMovement.players = playerMovements.ConvertAll(player => player.transform);
        }

        [Rpc(SendTo.Everyone)]
        public void PlayerDiedRpc(ulong player)
        {
            // match ulong to playerMovement
            var playerMovement = playerMovements.Find(w => w.OwnerClientId == player);
            if (diedPlayers.Contains(playerMovement))
                return;
            diedPlayers.Add(playerMovement);
            playerMovement.gameObject.SetActive(false);
            // Debug.Log($"{player.name} died.");
        }

        [Rpc(SendTo.Server)]
        public void RespawnPlayersRpc()
        {
            foreach (var player in diedPlayers)
            {
                player.transform.position = playerSpawnPoint.position;
                player.gameObject.SetActive(true);
                player.GetComponent<PlayerHealth>().Respawn();
            }
            diedPlayers.Clear();
        }
    }
}
