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

        public void SpawnInThisPlayer()
        {
            if (!IsOwner)
                return;
            Debug.Log($"Spawning in player {LobbyUtil.GetName(NetworkObject.OwnerClientId)}");
        }

        public void OnLeaving()
        {
            Destroy(gameObject);
        }
    }
}
