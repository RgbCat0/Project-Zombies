using System;
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

        [SerializeField]
        private int maxPlayers = 4;

        private Lobby _lobby;
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
            await UnityServices.InitializeAsync();
            await SignIn();
        }

        private async Task SignIn()
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            if (logLevel == LogLevel.All)
                Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
        }

        public async Task ChangeName(string newName)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
                if (logLevel == LogLevel.All)
                    Debug.Log($"Changed name to {newName}");
            }
            catch (Exception e)
            {
                if (logLevel is LogLevel.Error or LogLevel.All)
                    Debug.LogError($"Failed to change name: {e.Message}");
            }
        }

        private async void CreateLobby(string lobbyName, bool isPrivate)
        {
            try
            {
                var options = new CreateLobbyOptions { IsPrivate = isPrivate };
                _lobby = await LobbyService.Instance.CreateLobbyAsync(
                    lobbyName,
                    maxPlayers,
                    options
                );
                if (logLevel == LogLevel.All)
                    Debug.Log($"Created lobby {_lobby.Id}");
                await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            }
            catch (Exception e)
            {
                if (logLevel is LogLevel.Error or LogLevel.All)
                    Debug.LogError($"Failed to create lobby: {e.Message}");
            }
        }

        public QueryResponse GetLobbies()
        {
            try
            {
                return LobbyService.Instance.QueryLobbiesAsync().Result;
            }
            catch (Exception e)
            {
                if (logLevel is LogLevel.Error or LogLevel.All)
                    Debug.LogError($"Failed to get lobbies: {e.Message}");
                return null;
            }
        }
    }
}
