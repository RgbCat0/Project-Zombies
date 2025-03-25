using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

namespace _Scripts
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }
        public bool IsSignedIn { get; private set; }

        [SerializeField]
        private int maxPlayers = 4;

        private Lobby _lobby;
        private string _playerId;
        private string _joinCode;

        private enum LogLevel
        {
            All,
            Error,
            None
        }

        [SerializeField]
        private LogLevel logLevel = LogLevel.All;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private async void Start()
        {
            try
            {
                IsSignedIn = false;
                await UnityServices.InitializeAsync();
                await SignIn();
            }
            catch (Exception e)
            {
                if (logLevel != LogLevel.None)
                    Debug.LogException(e);
            }
        }

        private async Task SignIn()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (AuthenticationService.Instance.IsSignedIn)
                    IsSignedIn = true;
                _playerId = AuthenticationService.Instance.PlayerId;
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

        public async Task ChangeName(string newName)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
                if (logLevel is LogLevel.All)
                    Debug.Log($"Changed name to {newName}");
            }
            catch (Exception e)
            {
                if (logLevel is LogLevel.Error or LogLevel.All)
                    Debug.LogError($"Failed to change name: {e.Message}");
            }
        }

        public void CreateLobby(string lobbyName)
        {
            CreateLobbyP(lobbyName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>This void runs async.</remarks>
        /// <param name="lobbyName"></param>
        /// <param name="isPrivate"></param>
        private async void CreateLobbyP(string lobbyName, bool isPrivate = false)
        {
            try
            {
                var options = new CreateLobbyOptions { IsPrivate = isPrivate };
                _lobby = await LobbyService.Instance.CreateLobbyAsync(
                    lobbyName,
                    maxPlayers,
                    options
                );
                await RelayService.Instance.CreateAllocationAsync(maxPlayers);
                var callbacks = new LobbyEventCallbacks();
                callbacks.PlayerJoined += OnPlayerJoined;
                callbacks.KickedFromLobby += OnKickedFromLobby;
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(_lobby.Id, callbacks);
                if (logLevel == LogLevel.All)
                    Debug.Log($"Created lobby {_lobby.Name}");
                GetComponent<LobbyUi>().GoToLobby();
                await UpdatePlayerNameInLobby(); // unity multiplayer service does not automatically update player name so this is a weird fix
                GetComponent<LobbyUi>().OnNewPlayer(_lobby.Players);
            }
            catch (Exception e)
            {
                if (logLevel == LogLevel.None)
                    return;
                switch (e)
                {
                    case LobbyServiceException lobbyServiceException:
                        Debug.LogError($"Failed to create lobby: {e.Message}");
                        break;
                    case NullReferenceException nullReferenceException:
                        Debug.LogError(
                            $"Failed to find something while creating lobby: {e.Message}"
                        );
                        break;
                }
            }
        }

        private async Task UpdatePlayerNameInLobby()
        {
            string newName = AuthenticationService.Instance.PlayerName;
            string id = AuthenticationService.Instance.PlayerId;
            var newData = new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName",
                        new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, newName)
                    }
                }
            };

            await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, id, newData);
            if (logLevel is LogLevel.All)
                Debug.Log($"Updated player name in lobby to {newName}");
        }

        private void OnKickedFromLobby()
        {
            throw new NotImplementedException();
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> obj)
        {
            StartCoroutine(WaitForNameChange(obj[0]));
            // GetComponent<LobbyUi>().OnNewPlayer(obj[0].Player.Data["PlayerName"].Value);
        }

        private IEnumerator WaitForNameChange(LobbyPlayerJoined obj)
        {
            while (obj.Player?.Data == null)
                yield return null;
            GetComponent<LobbyUi>().OnNewPlayer(_lobby.Players);
        }

        public async Task<QueryResponse> GetLobbies()
        {
            try
            {
                //testing quick join
                // var test = await LobbyService.Instance.QuickJoinLobbyAsync();
                // Debug.Log($"Quick joined lobby: {test.Name}");
                // return null;
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

        public void JoinLobby(string lobbyId) => JoinLobbyP(lobbyId);

        private async void JoinLobbyP(string lobbyId)
        {
            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                await UpdatePlayerNameInLobby();
                GetComponent<LobbyUi>().GoToLobby();
                GetComponent<LobbyUi>().OnNewPlayer(_lobby.Players);

                if (logLevel == LogLevel.All)
                    Debug.Log($"Joined lobby {lobbyId}");
            }
            catch (Exception e)
            {
                if (logLevel != LogLevel.None)
                    Debug.LogError($"Failed to join lobby: {e.Message}");
            }
        }

        public void DisconnectLobby(string lobbyId) => DisconnectLobbyP(lobbyId);

        private async void DisconnectLobbyP(string lobbyId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, _playerId);
                if (logLevel == LogLevel.All)
                    Debug.Log($"Left lobby {lobbyId}");
            }
            catch (Exception e)
            {
                if (logLevel != LogLevel.None)
                    Debug.LogError($"Failed to leave lobby: {e.Message}");
            }
        }
    }
}
