using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace _Scripts.LobbyScripts
{
    public static class LobbyUtil // helper class for lobby
    {
        public static LobbyUi LobbyUi;

        public static async Task UpdatePlayerNameInLobby()
        {
            string newName = FormatName(AuthenticationService.Instance.PlayerName);
            string id = AuthenticationService.Instance.PlayerId;
            var newData = new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName",
                        new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, newName)
                    },
                },
            };

            await LobbyService.Instance.UpdatePlayerAsync(
                LobbyManager.Instance.Lobby.Id,
                id,
                newData
            );
            Log($"Updated player Data in lobby of user: {newName}");
        }

        private static string FormatName(string name)
        {
            // remove the #4040 from the end of the name
            name = name.Remove(name.IndexOf('#'));
            // first letter to uppercase
            return name[0].ToString().ToUpper() + name[1..];
        }

        public static async Task UpdateUlongIdInLobby()
        {
            string id = AuthenticationService.Instance.PlayerId;
            var newData = new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "UlongId",
                        new PlayerDataObject(
                            PlayerDataObject.VisibilityOptions.Public,
                            NetworkManager.Singleton.LocalClientId.ToString()
                        )
                    },
                },
            };

            await LobbyService.Instance.UpdatePlayerAsync(
                LobbyManager.Instance.Lobby.Id,
                id,
                newData
            );
            Log($"Updated ulong ID in lobby of user: {id}");
        }

        public static async Task ChangeName(string newName)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            }
            catch (Exception e)
            {
                Log($"Failed to change name: {e.Message}", LogType.Error);
            }
        }

        #region logging

        public static void Log(
            object message,
            LogType logType = LogType.Log,
            [CanBeNull] Exception exception = null
        )
        {
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Exception:
                case LogType.Assert:
                case LogType.Error:
                    Debug.LogError(message + exception?.StackTrace);
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

        public static string NetworkIdToLobbyId(ulong clientId)
        {
            return LobbyManager
                .Instance.Lobby.Players.Find(w => w.Data["UlongId"].Value == clientId.ToString())
                .Id;
        }

        public static ulong LobbyIdToNetworkId(string clientId)
        {
            var test = LobbyManager
                .Instance.Lobby.Players.Find(w => w.Id == clientId)
                .Data["UlongId"]
                .Value;
            Debug.LogWarning(test);
            return Convert.ToUInt64(test);
        }

        private static string UlongIdToPlayerName(ulong clientId)
        {
            var player = LobbyManager.Instance.ConvertedIds[clientId];
            var playerName = LobbyManager
                .Instance.Lobby.Players.Find(w => w.Id == player)
                .Data["PlayerName"]
                .Value;
            return playerName;
        }

        public static string GetName(string id)
        {
            var player = LobbyManager.Instance.Lobby.Players.Find(w => w.Id == id);
            Debug.LogWarning(player is null);
            return player.Data["PlayerName"].Value;
        }

        public static string GetName(ulong id) => UlongIdToPlayerName(id);
    }
}
