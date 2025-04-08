using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using static _Scripts.LobbyUtil;

namespace _Scripts
{
    public static class LobbyNetworking
    {
        #region Host
        public static async Task<(Allocation, string, RelayServerData)> StartHostAllocation(
            int maxPlayers
        )
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(
                    maxPlayers
                );
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(
                    allocation.AllocationId
                );
                RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
                return (allocation, relayJoinCode, relayServerData);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to create relay allocation: " + e.Message);
                Status("Failed to create relay allocation.", Color.red);
                throw;
            }
        }

        public static async Task<Lobby> StartHostLobby(
            bool isPrivate,
            string lobbyName,
            string relayJoinCode
        )
        {
            try
            {
                var options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            "relayJoinCode",
                            new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode)
                        }
                    }
                };
                return await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, options);
            }
            catch (Exception e) // proper error handling
            {
                Debug.LogError($"Failed to create lobby: {e.Message}");
                Status("Failed to create lobby.", Color.red);
                throw;
            }
        }
        #endregion
        #region Client

        public static async Task<RelayServerData> StartClientAllocation(string code)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(
                    code
                );
                return joinAllocation.ToRelayServerData("dtls");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join relay: {e.Message}");
                Status("Failed to join relay.", Color.red);
                throw;
            }
        }

        public static async Task<Lobby> StartClientLobby(string lobbyId)
        {
            try
            {
                return await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join lobby: {e.Message}");
                Status("Failed to join lobby.", Color.red);
                throw;
            }
        }
        #endregion

        public static void StartNetworking(RelayServerData serverData, bool isHost)
        {
            try
            {
                NetworkManager
                    .Singleton.GetComponent<UnityTransport>()
                    .SetRelayServerData(serverData);
                if (isHost)
                    NetworkManager.Singleton.StartHost();
                else
                    NetworkManager.Singleton.StartClient();
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Failed to start Unity Networking as {(isHost ? "host" : "client")}: {e.Message}"
                );
                Status("Failed to start Unity Networking.", Color.red);
                throw;
            }
        }
    }
}
