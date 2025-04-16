using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using _Scripts.LobbyScripts;

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
            UpdateNameRpc();
            // Debug.Log($"Spawning in player {playerName}");
        }

        [Rpc(SendTo.Everyone)]
        private void UpdateNameRpc()
        {
            var playerName = LobbyUtil.GetName(_lobbyPlayerId);
            NetworkObject.name = $"Player {playerName}";
            inGamePlayer.name = $"InGamePlayer {playerName}";
            inGamePlayer.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                playerName;
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

        public override void OnDestroy()
        {
            if (!IsOwner)
                return;
            SceneManager.MoveGameObjectToScene(
                transform.parent.gameObject,
                SceneManager.GetActiveScene()
            );
            base.OnDestroy();
        }

        public void OnLeaving()
        {
            Destroy(gameObject);
        }
    }
}
