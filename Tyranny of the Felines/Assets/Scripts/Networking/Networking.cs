using Matryoshka.Http;
using Matryoshka.Lobby;
using Matryoshka.UI;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Matryoshka.Networking {

    /// <summary>
    /// RelayHostData represents the necessary informations
    /// for a Host to host a game on a Relay
    /// </summary>
    public struct RelayData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
        public byte[] HostConnectionData;
    }

    public enum LobbyType
    {
        LAN, // LAN/Portforwarded
        Direct, // Port is open
        Relay, // Use Unity's relay service
        Punch, // UDP holepunching
    }

    public class Networking : MonoBehaviour
    {
        private static Networking _singleton;
        public static Networking Singleton {
            get
            {
                if (_singleton == null)
                {
                    GameObject go = new GameObject();
                    _singleton = go.AddComponent<Networking>();
                    go.name = "NetworkingSingleton";
                }
                return _singleton;
            }
            private set
            {
                _singleton = value;
            }
        }
        public Networking()
        {
            if (_singleton != null)
            {
                throw new Exception("Don't re-construct the network singleton!");
            }
        }

        public async Task<bool> InitializeNetworking()
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                //If not already logged, log the user in
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            return AuthenticationService.Instance.IsSignedIn;
        }

        public async Task<bool> HostGame(LobbyType type, string name="unnamed")
        {
            Debug.Log($"Hosting game type: {type}");
            // TODO punch.
            switch (type) {
                case LobbyType.Relay:
                    if (!await InitializeNetworking())
                    {
                        ModalUI.Singleton.ShowMessageModal("Relay Fail", "Failed to authenticate with unity services.");
                        return false;
                    }
                    try
                    {
                        RelayData data = await HostRelay();
                        HttpClient.Singleton.HostServer(data.JoinCode, name);
                        SoundManager.Singleton.PlayCharacterMusic();
                    } catch
                    {
                        ModalUI.Singleton.ShowMessageModal("You do not have permission.", "You do not have permission to use the relay service with this build.");
                        return false;
                    }
                    break;
                case LobbyType.LAN:
                case LobbyType.Direct:
                    string ip = type == LobbyType.Direct ? Networking.GetPublicIpAddress() : Networking.GetLocalIpAddress();
                    int port = Networking.GetAvailablePort();
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0.0.0.0", (ushort)port);
                    HttpClient.Singleton.HostServer(ip, port, name);
                    SoundManager.Singleton.PlayCharacterMusic();
                    break;
                default:
                    // The code still exists on the network_punch branch but to be properly integrated with unity
                    // a custon NetworkTransport has to be implemented and we never got it working.
                    ModalUI.Singleton.ShowMessageModal("Lobby type unsupported.", "UDP holepunching is disabled in this build, due to inconsistent behaviour.");
                    return false;
            }

            if (NetworkManager.Singleton.StartHost())
            {
                ModalUI.Singleton.ShowMessageModalWithoutButtons("Hosting", "Attempting to host a lobby.");
                StartCoroutine("JoinTimeout", "Failed to host Lobby. Check your network configuration and try restarting your game.");
            }
            else
            {
                NetworkManager.Singleton.Shutdown();
                ModalUI.Singleton.ShowMessageModal("Failed to host server.", "Check your network configuration and try restarting your game.");
                return false;
            }

            return true;
        }

        public async Task<RelayData> HostRelay()
        {
            Allocation allocation = await Relay.Instance.CreateAllocationAsync(3);
            RelayData data = new RelayData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };
            data.JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            transport.SetRelayServerData(data.IPv4Address, data.Port, data.AllocationIDBytes, data.Key, data.ConnectionData);
            return data;
        }

        public async void JoinServer(Server.Server server)
        {
            switch (server.lobby_type)
            {
                case "Direct":
                    Debug.Log($"Joining direct {server.ip}:{server.port}");
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(server.ip, server.port);
                    SoundManager.Singleton.PlayCharacterMusic();
                    //JoinDirect(server.ip, server.port);
                    break;
                case "Relay":
                    Debug.Log($"Joining relay {server.join_code}");
                    try
                    {
                        await JoinRelay(server.join_code);

                    } catch
                    {
                        ModalUI.Singleton.ShowMessageModal("Failed to join the lobby.", "Lobby does not exist, or you don't have permission to join.");
                        return;
                    }
                    break;
                default:
                    Debug.LogError($"Failed to join: Unknown server type: {server.lobby_type}");
                    ModalUI.Singleton.ShowMessageModal("Failed to join lobby.", "Unknown server type.");
                    break;
            }

            if (NetworkManager.Singleton.StartClient())
            {
                ModalUI.Singleton.ShowMessageModalWithoutButtons("Joining", "Attempting to join a lobby.");
                StartCoroutine("JoinTimeout", "Failed to join the lobby. Lobby is full or doesn't exist anymore.");
            }
            else
            {
                ModalUI.Singleton.ShowMessageModal("Failed to join the lobby.", "Lobby probably does not exist anymore.");
            }
        }

        public async Task<RelayData> JoinRelay(string joinCode)
        {
            if (!await InitializeNetworking())
            {
                ModalUI.Singleton.ShowMessageModal("Relay Fail", "Failed to authenticate with unity services.");
            }
            JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

            //Populate the joining data
            RelayData data = new RelayData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };

            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            transport.SetRelayServerData(data.IPv4Address, data.Port, data.AllocationIDBytes, data.Key, data.ConnectionData, data.HostConnectionData);
            return data;
        }

        private IEnumerator JoinTimeout(string failureMessage)
        {
            for (int i = 0; i < Constants.NumRetries; i++)
            {
                Debug.Log("Timeout");
                if (LobbyManager.Singleton == null)
                {
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.lobby);
                    ModalUI.Singleton.CloseModal();
                    SoundManager.Singleton.PlayCharacterMusic();
                    yield break;
                }
            }
            NetworkManager.Singleton.Shutdown();
            ModalUI.Singleton.ShowMessageModal("Connection Failure", failureMessage);
            yield break;
        }

        public static string GetLocalIpAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 12345);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        public static string GetPublicIpAddress()
        {
            return new System.Net.WebClient().DownloadString("https://api.ipify.org");
        }

        public static int GetAvailablePort()
        {
            try
            {
                return Enumerable.Range(37000, 38000).First(port => !IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().Any(listener => listener.Port == port));
            }
            catch
            {
                return UnityEngine.Random.Range(37000, 38000);
            }
        }
    }
}
