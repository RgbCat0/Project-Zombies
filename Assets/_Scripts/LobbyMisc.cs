using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace _Scripts
{
    public static class LobbyMisc // helper class for lobby
    {
        public static async Task UpdatePlayerNameInLobby()
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

            await LobbyService.Instance.UpdatePlayerAsync(
                LobbyManager.Instance.Lobby.Id,
                id,
                newData
            );
            if (LobbyManager.Instance.logLevel is LobbyManager.LogLevel.All)
                Debug.Log($"Updated player name in lobby to {newName}");
        }

        public static async Task ChangeName(string newName)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
                if (LobbyManager.Instance.logLevel is LobbyManager.LogLevel.All)
                    Debug.Log($"Changed name to {newName}");
            }
            catch (Exception e)
            {
                if (
                    LobbyManager.Instance.logLevel
                    is LobbyManager.LogLevel.Error
                        or LobbyManager.LogLevel.All
                )
                    Debug.LogError($"Failed to change name: {e.Message}");
            }
        }
    }
}
