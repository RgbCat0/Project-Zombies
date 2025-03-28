using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts
{
    public class LobbyManager : NetworkBehaviour
    {
        public static LobbyManager Instance { get; private set; }
        public bool IsSignedIn { get; private set; }
        private bool GameStarted { get; set; }

        [SerializeField]
        private int maxPlayers = 4;

        public Lobby Lobby { get; private set; }
        private string _playerId;
        private ulong _playerClientId;
        private string _joinCode;

        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField,]
        private List<GameObject> playerObjects;

        public enum LogLevel
        {
            Verbose,
            All,
            Error,
            None
        }

        public LogLevel logLevel = LogLevel.Verbose;

        #region Insance
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
        #region Initialization
        private async void Start()
        {
            try
            {
                IsSignedIn = false;
                NetworkManager.Singleton.ConnectionApprovalCallback += ApproveConnection;
                await UnityServices.InitializeAsync();
                await SignIn();
            }
            catch (Exception e)
            {
                if (logLevel != LogLevel.None)
                    Debug.LogException(e);
            }
        }

        private void LogIt(string log, LogLevel chosen, LogType logType)
        {
            // call this method to choose correct log level (example if loglevel == error then only error logs will be shown and if verbose show verbose, all and error)
            if (logLevel <= chosen)
                Debug.Log(log);
        }

        private async Task SignIn()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (AuthenticationService.Instance.IsSignedIn)
                    IsSignedIn = true;
                _playerId = AuthenticationService.Instance.PlayerId;
                _playerClientId = NetworkManager.Singleton.LocalClientId;
            }
            catch (Exception e)
            {
                IsSignedIn = false;
                if (logLevel != LogLevel.None)
                    Debug.LogError($"Sign in failed: {e.Message}");
            }
            if (logLevel == LogLevel.All)
                Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerName}");
        }
        #endregion
        #region Creating
        /// <summary>
        /// This method creates a lobby and makes this player the host.
        /// </summary>
        /// <exception cref="LobbyServiceException">The lobby service itself errored.</exception>
        /// <exception cref="NullReferenceException">Something is not assigned.</exception>
        /// <remarks>This is a public void which calls a private async method.</remarks>
        /// <param name="lobbyName">Name for the lobby</param>
        public void CreateLobby(string lobbyName)
        {
            CreateLobbyP(lobbyName);
        }

        private async void CreateLobbyP(string lobbyName, bool isPrivate = false)
        {
            try
            {
                var options = new CreateLobbyOptions { IsPrivate = isPrivate };
                Lobby = await LobbyService.Instance.CreateLobbyAsync(
                    lobbyName,
                    maxPlayers,
                    options
                );
                await RelayService.Instance.CreateAllocationAsync(maxPlayers);
                NetworkManager.Singleton.StartHost();
                Debug.Log(IsServer);
                var callbacks = new LobbyEventCallbacks();
                callbacks.PlayerJoined += OnPlayerJoined;
                callbacks.PlayerLeft += OnPlayerLeft;
                callbacks.KickedFromLobby += OnKickedFromLobby;
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(Lobby.Id, callbacks);
                if (logLevel == LogLevel.All)
                    Debug.Log($"Created lobby {Lobby.Name}");
                GetComponent<LobbyUi>().GoToLobby();
                await LobbyMisc.UpdatePlayerNameInLobby(); // unity multiplayer service does not automatically update player name so this is a weird fix
                GetComponent<LobbyUi>().OnNewPlayer(Lobby.Players);
                SpawnNewPlayerObjectRpc(_playerClientId);
            }
            catch (Exception e)
            {
                if (logLevel == LogLevel.None)
                    return;
                switch (e)
                {
                    case LobbyServiceException:
                        Debug.LogError($"Failed to create lobby: {e.Message}");
                        break;
                    case NullReferenceException:
                        Debug.LogError(
                            $"Failed to find something while creating lobby: {e.Message}"
                        );
                        break;
                }
            }
        }
        #endregion
        #region Joining
        public async Task<QueryResponse> GetLobbies()
        {
            try
            {
                var test = await LobbyService.Instance.QueryLobbiesAsync();

                if (logLevel == LogLevel.All)
                    Debug.Log($"Got lobbies: {test.Results.Count}");
                return test;
            }
            catch (Exception e)
            {
                if (logLevel is LogLevel.Error or LogLevel.All)
                    Debug.LogError($"Failed to get lobbies: {e.Message}");
                return null;
            }
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> obj)
        {
            StartCoroutine(WaitForNameChange(obj[0]));
            // GetComponent<LobbyUi>().OnNewPlayer(obj[0].Player.Data["PlayerName"].Value);
        }

        [Rpc(SendTo.Server)]
        private void SpawnNewPlayerObjectRpc(ulong playerUlongId)
        {
            var newPlayer = Instantiate(playerPrefab);
            playerObjects.Add(newPlayer);
            newPlayer.GetComponent<Player.Player>().OnCreated(playerUlongId);
        }

        public void JoinLobby(string lobbyId) => JoinLobbyP(lobbyId);

        private async void JoinLobbyP(string lobbyId) // Client Joining
        {
            try
            {
                Lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                await LobbyMisc.UpdatePlayerNameInLobby();
                GetComponent<LobbyUi>().GoToLobby();
                GetComponent<LobbyUi>().OnNewPlayer(Lobby.Players);

                if (logLevel == LogLevel.All)
                    Debug.Log($"Joined lobby {lobbyId}");
            }
            catch (Exception e)
            {
                if (logLevel != LogLevel.None)
                    Debug.LogError($"Failed to join lobby: {e.Message}");
            }
        }
        #endregion
        #region Disconnection
        public void DisconnectLobby(string lobbyId) => DisconnectLobbyP(lobbyId);

        private async void DisconnectLobbyP(string lobbyId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, _playerId);
                if (logLevel == LogLevel.All)
                    Debug.Log($"Left lobby {lobbyId}");
            }
            catch (Exception e)
            {
                if (logLevel != LogLevel.None)
                    Debug.LogError($"Failed to leave lobby: {e.Message}");
            }
        }

        private void OnKickedFromLobby()
        {
            throw new NotImplementedException();
        }

        private void OnPlayerLeft(List<int> wot)
        {
            // output list of int to json in debug log
            Debug.Log(JsonUtility.ToJson(wot)); // (because i have no idea what it gives.)
        }

        #endregion
        #region GameStarted

        public void StartGame() => StartGameP();

        private void StartGameP()
        {
            try
            {
                if (NetworkManager.Singleton.IsServer)
                    GameStarted = true;

                SceneManager.LoadScene("Main");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to start game: {e.Message}");
            }
        }
        #endregion
        #region Misc
        private IEnumerator WaitForNameChange(LobbyPlayerJoined obj)
        {
            while (obj.Player?.Data == null)
                yield return null;
            GetComponent<LobbyUi>().OnNewPlayer(Lobby.Players);
        }

        private void ApproveConnection(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response
        )
        {
            if (GameStarted)
            {
                response.Approved = false;
                response.Reason = "Game has already started.";
                return;
            }
            response.Approved = true;
        }
        #endregion
    }
}
