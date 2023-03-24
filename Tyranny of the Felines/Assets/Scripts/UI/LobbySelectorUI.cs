using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Http;
using Matryoshka.Lobby;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace Matryoshka.UI
{
    public class LobbySelectorUI : MonoBehaviour
    {
        public static LobbySelectorUI Singleton { get; private set; }
        private List<Server.Server> servers;
        [SerializeField]
        private GameObject lobbyManagerPrefab;
        private Button hostLobbyRelayButton;
        private Button hostLobbyLANButton;
        //private Button hostLobbyPeerButton;
        private Button refreshLobbiesButton;
        private Button mainMenuButton;
        private Button manualJoinButton;
        private TextField hostLobbyName;
        private ScrollView lobbies;
        void Start()
        {
            Singleton = this;

            servers = new List<Server.Server>();
        
            var root = GetComponent<UIDocument>().rootVisualElement;

            hostLobbyRelayButton = root.Q<Button>(Constants.HostLobbyRelayButton);
            hostLobbyLANButton = root.Q<Button>(Constants.HostLobbyLANButton);
            //hostLobbyPeerButton = root.Q<Button>(Constants.HostLobbyPeerButton);
            refreshLobbiesButton = root.Q<Button>(Constants.RefreshLobbyButton);
            mainMenuButton = root.Q<Button>(Constants.MainMenuButton);
            manualJoinButton = root.Q<Button>(Constants.ManualJoinButton);
            hostLobbyName = root.Q<TextField>(Constants.HostLobbyName);

            hostLobbyRelayButton.clicked += () => HostLobbyButtonPressed(Networking.LobbyType.Relay);
            hostLobbyLANButton.clicked += () => HostLobbyButtonPressed(Networking.LobbyType.LAN);
            //hostLobbyPeerButton.clicked += () => HostLobbyButtonPressed(Networking.LobbyType.Punch);
            refreshLobbiesButton.clicked += RefreshLobbiesButtonPressed;
            mainMenuButton.clicked += MainMenuButtonPressed;
            manualJoinButton.clicked += () => ModalUI.Singleton.ShowInputModal("Direct Connection: IP:Port", null, OnDirectConnect);

            lobbies = root.Q<ScrollView>(Constants.Lobbies);
        }

        void OnDirectConnect(string input)
        {
            string ip = input.Split(':')[0];
            int port = int.Parse(input.Split(':')[1]);
            Debug.Log($"Direct connect {input} {ip}:{port}");
            Server.Server server = new Server.Server("", "", 0, ip, port);

            LobbyStart.leftLobby = false;
            Networking.Networking.Singleton.JoinServer(server);
        }

        private async void HostLobbyButtonPressed(Networking.LobbyType type)
        {
            SoundManager.Singleton.PlayMenuClick();
            Instantiate(lobbyManagerPrefab, Vector3.zero, Quaternion.identity);
            await Networking.Networking.Singleton.HostGame(type, hostLobbyName.value);
        }
    
        void RefreshLobbiesButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            HttpClient.Singleton.GetServers();
        }

        void MainMenuButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.mainMenu);
        }

        public void RenderServers(List<Server.Server> serversToRender) 
        {
            lobbies.Clear();
            if(serversToRender.Count == 0) 
            {
                lobbies.Add(CreateEmptyServerCard());
            }
            else
            {
                foreach (Server.Server server in serversToRender) 
                {
                    lobbies.Add(CreateServerCard(server));
                }
            }
        }
    
    private VisualElement CreateServerCard(Server.Server server) 
    {
        VisualElement serverCard = new VisualElement();
        Label serverName = new Label();
        serverName.text = server.name;
        Label playerCount = new Label();
        playerCount.text = $"({server.players}/{Constants.MaxPlayers})";
        Button joinButton = new Button();
        joinButton.text = "Join " + server.lobby_type;
        joinButton.clicked += () => {
            if(server.players < Constants.MaxPlayers)
            {
                LobbyStart.leftLobby = false;
                Debug.Log($"Trying to join {server.lobby_type}");
                Networking.Networking.Singleton.JoinServer(server);
            }
            else{
                ModalUI.Singleton.ShowMessageModal("Failed to join the lobby.", "Lobby is full");
            }
            
        };
        serverCard.Add(serverName);
        serverCard.Add(playerCount);
        serverCard.Add(joinButton);
        return serverCard;
    }
    
        private VisualElement CreateEmptyServerCard()
        {
            VisualElement serverCard = new VisualElement();
            Label text = new Label();
            text.text = "No lobbies are currently being hosted";
            serverCard.Add(text);
            return serverCard;
        }
    }
}
