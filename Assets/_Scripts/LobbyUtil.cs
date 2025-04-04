using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace _Scripts
{
    public static class LobbyUtil // helper class for lobby
    {
        public static LobbyUi LobbyUi;

        public static async Task UpdatePlayerDataInLobby()
        {
            string newName = AuthenticationService.Instance.PlayerName;
            string id = AuthenticationService.Instance.PlayerId;
            // remove the #4040 from the end of the name
            newName = newName.Remove(newName.IndexOf('#'));
            // first letter to uppercase
            newName = newName[0].ToString().ToUpper() + newName[1..];
            var playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            Debug.Log(NetworkManager.Singleton.IsConnectedClient);
            ulong clientId = NetworkManager.Singleton.ConnectedClientsIds[0]; // gets the newest player (hack fix)
            var newData = new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName",
                        new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, newName)
                    },
                    {
                        "UlongId",
                        new PlayerDataObject(
                            PlayerDataObject.VisibilityOptions.Public,
                            clientId.ToString()
                        )
                    }
                }
            };

            await LobbyService.Instance.UpdatePlayerAsync(
                LobbyManager.Instance.Lobby.Id,
                id,
                newData
            );
            Log($"Updated player Data in lobby of user: {newName}");
        }

        public static async Task ChangeName(string newName)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
                Log($"Changed name to {newName}");
            }
            catch (Exception e)
            {
                Log($"Failed to change name: {e.Message}", LogType.Error);
            }
        }

        #region logging

        public static void Log(
            string text,
            LogType logType = LogType.Log,
            [CanBeNull] Exception exception = null
        )
        {
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(text);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(text);
                    break;
                case LogType.Error:
                    Debug.LogError(text + exception?.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }

        /// <summary>
        /// Changes the status in the lobby UI
        /// </summary>
        /// <param name="text">Specifies what should be shown to the player, if left blank nothing will be shown.</param>
        /// <param name="color">Changes the text color (optional), defaults to white.</param>
        public static void Status(string text = "", Color color = default)
        {
            LobbyUi.ChangeStatus(text, color);
        }
        #endregion

        public static string NetworkManagerClientIdToLobbyClientId(ulong clientId)
        {
            return LobbyManager
                .Instance.Lobby.Players.Find(w => w.Data["UlongId"].Value == clientId.ToString())
                .Id;
        }

        public static ulong LobbyClientIdToNetworkManagerClientId(string clientId)
        {
            var test = LobbyManager
                .Instance.Lobby.Players.Find(w => w.Id == clientId)
                .Data["UlongId"]
                .Value;
            Debug.LogWarning(test);
            return Convert.ToUInt64(test);
        }

        private static string NetworkManagerClientIdToPlayerName(ulong clientId)
        {
            var player = LobbyManager.Instance.Lobby.Players.Find(w =>
                w.Data["UlongId"].Value == clientId.ToString()
            );
            return player.Data["PlayerName"].Value;
        }

        public static string GetName(string id)
        {
            var player = LobbyManager.Instance.Lobby.Players.Find(w => w.Id == id);
            return player.Data["PlayerName"].Value;
        }

        public static string GetName(ulong id) => NetworkManagerClientIdToPlayerName(id);
    }
}
