using Matryoshka.Entity;
using Matryoshka.Game;
using Matryoshka.Http;
using Matryoshka.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Matryoshka.Lobby
{
    public class LobbyStart : MonoBehaviour
    {
        public static bool leftLobby = false;

        public void Start()
        {
            Debug.Log("Lobby Start");
            // Add Methods to the Network Manager to deal with connections and disconnections
            NetworkManager.Singleton.ConnectionApprovalCallback += ClientApprove;
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnect;
        }
        
        public void Stop()
        {
            Debug.Log("Lobby Stop");
            // Add Methods to the Network Manager to deal with connections and disconnections
            NetworkManager.Singleton.ConnectionApprovalCallback -= ClientApprove;
            NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnect;
        }

        private void ClientApprove(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            bool approve = NetworkManager.Singleton.ConnectedClients.Count < Constants.MaxPlayers;
            bool createPlayerObject = false;
        
            callback(createPlayerObject, null, approve, Vector3.zero, Quaternion.identity);
        }
    
        private void ClientConnect(ulong clientId)
        {
            Debug.Log($"Client Connect Fired: {clientId}");
            if (NetworkManager.Singleton.IsServer)
            {
                LobbyManager.Singleton.SpawnIfUnspawned();
                LobbyManager.Singleton.Players.Add(new LobbyPlayer(clientId, "Player " + clientId, PlayerClass.None));
                if (LobbyManager.Singleton.Players.Count > 1)
                {
                    HttpClient.Singleton.UpdateServerPlayerCount(LobbyManager.Singleton.Players.Count);
                }
                
            }
        
        }
    
        private void ClientDisconnect(ulong clientId)
        {
            // Find disconnecting client and remove them
            Debug.Log($"Client Disconnect Fired: {clientId}");
            if (NetworkManager.Singleton.IsServer)
            {
                for(int i = 0; i < LobbyManager.Singleton.Players.Count; i++)
                {
                    LobbyPlayer player = LobbyManager.Singleton.Players[i];
                    if(player.GetId() == clientId) 
                    {
                        LobbyManager.Singleton.Players.RemoveAt(i);
                        if (GameManager.Singleton != null)
                        {
                            HttpClient.Singleton.UpdateServerPlayerCount(LobbyManager.Singleton.Players.Count);
                        }
                        break;
                    }
                }
            }
            else
            {
                NetworkManager.Singleton.Shutdown();
                HttpClient.Singleton.GetServers();
                GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.lobbySelector);
                if(!leftLobby)
                {
                    ModalUI.Singleton.ShowMessageModal("Host disconnected", "The host has stopped hosting the game. Returning you to the lobby selector.");
                    SoundManager.Singleton.PlayMenuMusic();
                    SceneManager.LoadScene("MainMenu");
                }
            }

            if(leftLobby)
            {
                leftLobby = false;
            }
        }
    
    
    }
}
