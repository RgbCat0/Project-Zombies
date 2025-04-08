using _Scripts.LobbyScripts;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace _Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField]
        private NetworkObject inGamePlayerPrefab;

        [SerializeField]
        private string _lobbyPlayerId;

        public NetworkObject inGamePlayer;

        private void Start()
        {
            if (!IsOwner)
            {
                return;
            }
            UpdatePlayerIdLate();
            ParentThisRpc();
            SendUlongIdToServerRpc(_lobbyPlayerId);
            // NetworkManager.SceneManager.OnLoadComplete += GameManager.Instance.SceneManagerOnOnLoadComplete;
        }

        [Rpc(SendTo.Server)]
        private void ParentThisRpc()
        {
            transform.parent = GameObject.Find("PlayerParent").transform;
            LobbyManager.Instance.CheckForPlayersRpc();
            DDolThisRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void DDolThisRpc()
        {
            DontDestroyOnLoad(transform.parent.gameObject);
        }

        [Rpc(SendTo.Everyone)]
        private void SendUlongIdToServerRpc(string id)
        {
            _lobbyPlayerId = id;
            ulong ulongId = NetworkObject.OwnerClientId;
            Debug.Log($"Player {_lobbyPlayerId} joined.");
            LobbyManager.Instance.ConvertedIds.Add(ulongId, _lobbyPlayerId);
        }

        public void SpawnInThisPlayer()
        {
            inGamePlayer = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                inGamePlayerPrefab,
                NetworkObject.OwnerClientId,
                isPlayerObject: true
            );
            var playerName = LobbyUtil.GetName(_lobbyPlayerId);
            NetworkObject.name = $"Player {playerName}";
            inGamePlayer.name = $"InGamePlayer {playerName}";
            Debug.Log($"Spawning in player {playerName}");
        }

        public void UpdatePlayerIdLate()
        {
            if (!IsOwner)
                return;
            _lobbyPlayerId = AuthenticationService.Instance.PlayerId;
            UpdaterRpc(_lobbyPlayerId);
        }

        [Rpc(SendTo.Everyone)]
        private void UpdaterRpc(string id)
        {
            _lobbyPlayerId = id;
        }

        public void OnLeaving()
        {
            Destroy(gameObject);
        }
    }
}
