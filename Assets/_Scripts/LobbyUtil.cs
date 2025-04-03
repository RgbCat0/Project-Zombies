using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace _Scripts
{
    public static class LobbyUtil // helper class for lobby
    {
        public static LobbyUi LobbyUi;

        public static async Task UpdatePlayerNameInLobby()
        {
            string newName = AuthenticationService.Instance.PlayerName;
            string id = AuthenticationService.Instance.PlayerId;
            // remove the #4040 from the end of the name
            newName = newName.Remove(newName.IndexOf('#'));
            // first letter to uppercase
            newName = newName[0].ToString().ToUpper() + newName[1..];
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
            Log($"Updated player name in lobby to {newName}");
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
    }
}
