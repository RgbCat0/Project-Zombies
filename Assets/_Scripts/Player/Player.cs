using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace _Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField]
        private NetworkObject inGamePlayerPrefab;
        private string _lobbyPlayerId;

        private void Start()
        {
            if (!IsOwner)
            {
                return;
            }
            _lobbyPlayerId = AuthenticationService.Instance.PlayerId;
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
        private void SendUlongIdToServerRpc(string playerId)
        {
            _lobbyPlayerId = playerId;
            ulong ulongId = NetworkObject.OwnerClientId;
            Debug.Log($"Player {playerId} joined.");
            LobbyManager.Instance.ConvertedIds.Add(ulongId, playerId);
        }

        public void SpawnInThisPlayer()
        {
            var test = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                inGamePlayerPrefab,
                NetworkObject.OwnerClientId,
                isPlayerObject: true
            );
            var playerName = LobbyUtil.GetName(_lobbyPlayerId);
            NetworkObject.name = $"Player {playerName}";
            Debug.Log($"Spawning in player {playerName}");
        }

        public void OnLeaving()
        {
            Destroy(gameObject);
        }
    }
}
