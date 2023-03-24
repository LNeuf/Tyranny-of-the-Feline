using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Game;
using Matryoshka.Http;
using Matryoshka.Lobby;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Matryoshka.UI
{
    public class LobbyUI : MonoBehaviour
    {
        private const float IntroVideoDuration = 101f;
        private const float Escape = 1f;
        private bool isWatchingVideo = false;
        public static LobbyUI Singleton { get; private set; }
        private Button leaveLobbyButton;
        private Button startGameButton;
        private Button bigSelectButton;
        private Button wazoSelectButton;
        private Button salumonSelectButton;
        private Button deselectCharacterButton;
        private Label bigPlayerName;
        private Label wazoPlayerName;
        private Label salumonPlayerName;
        void Start()
        {
            Singleton = this;
        
            var root = GetComponent<UIDocument>().rootVisualElement;
        
            bigPlayerName = root.Q<Label>(Constants.BigPlayerName);
            wazoPlayerName = root.Q<Label>(Constants.WazoPlayerName);
            salumonPlayerName = root.Q<Label>(Constants.SalumonPlayerName);
        
            leaveLobbyButton = root.Q<Button>(Constants.LeaveLobbyButton);
            startGameButton = root.Q<Button>(Constants.StartGameButton);
            bigSelectButton = root.Q<Button>(Constants.BigSelectButton);
            wazoSelectButton = root.Q<Button>(Constants.WazoSelectButton);
            salumonSelectButton = root.Q<Button>(Constants.SalumonSelectButton);
            deselectCharacterButton = root.Q<Button>(Constants.DeselectCharacterButton);
        
            leaveLobbyButton.clicked += LeaveLobbyButtonPressed;
            startGameButton.clicked += StartGameButtonPressed;
            bigSelectButton.clicked += BigSelectButtonPressed;
            wazoSelectButton.clicked += WazoSelectButtonPressed;
            salumonSelectButton.clicked += SalumonSelectButtonPressed;
            deselectCharacterButton.clicked += DeselectCharacterButtonPressed;
        }

        private void FixedUpdate()
        {
            if (GameManager.Singleton == null && isWatchingVideo && Math.Abs(Input.GetAxis("Cancel") - Escape) < 0.0001)
            {
                SkipCutscene();
            }
        }

        void BigSelectButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            if (NetworkManager.Singleton.IsServer) 
            {
                LobbyManager.Singleton.SetCharacter(NetworkManager.Singleton.LocalClientId, PlayerClass.Mouse);
            }
            else 
            {
                LobbyManager.Singleton.RequestSetCharacterServerRpc(NetworkManager.Singleton.LocalClientId, PlayerClass.Mouse);
            }
        }
    
        void WazoSelectButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            if (NetworkManager.Singleton.IsServer) 
            {
                LobbyManager.Singleton.SetCharacter(NetworkManager.Singleton.LocalClientId, PlayerClass.Bird);
            }
            else 
            {
                LobbyManager.Singleton.RequestSetCharacterServerRpc(NetworkManager.Singleton.LocalClientId, PlayerClass.Bird);
            }
        }
    
        void SalumonSelectButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            if (NetworkManager.Singleton.IsServer) 
            {
                LobbyManager.Singleton.SetCharacter(NetworkManager.Singleton.LocalClientId, PlayerClass.Fish);
            }
            else 
            {
                LobbyManager.Singleton.RequestSetCharacterServerRpc(NetworkManager.Singleton.LocalClientId, PlayerClass.Fish);
            }
        }
    
        void DeselectCharacterButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            if (NetworkManager.Singleton.IsServer) 
            {
                LobbyManager.Singleton.SetCharacter(NetworkManager.Singleton.LocalClientId, PlayerClass.None);
            }
            else 
            {
                LobbyManager.Singleton.RequestSetCharacterServerRpc(NetworkManager.Singleton.LocalClientId, PlayerClass.None);
            }
        }
    
        void LeaveLobbyButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            LobbyStart.leftLobby = true;
            if(NetworkManager.Singleton.IsServer)
            {
                LobbyManager.Singleton.GetComponent<NetworkObject>().Despawn();
                HttpClient.Singleton.DeleteServer();
            }
            NetworkManager.Singleton.Shutdown();
            SoundManager.Singleton.PlayMenuMusic();
            GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.lobbySelector);
            //SceneManager.LoadScene("MainMenu");
        }
    
        void StartGameButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            if(NetworkManager.Singleton.IsServer)
            {
                if (LobbyManager.Singleton.Validate())
                {
                    Debug.Log("Starting the Game...");
                    HttpClient.Singleton.DeleteServer();
                    isWatchingVideo = true;
                    LobbyManager.Singleton.StartVideoClientRpc();
                    CutsceneOverlayUI.Singleton.ShowCutsceneOverlayUI();
                    StartCoroutine(CutsceneTimeout());
                }
                else
                {
                    ModalUI.Singleton.ShowMessageModal("Cannot start the game", "All players in the lobby must select a class before starting the game.");
                }
            }
        }

        public void SkipCutscene()
        {
            StopAllCoroutines();
            StartGame();
            LobbyManager.Singleton.PlaySoundClientRpc("PlayBossMusic");
        }

        public IEnumerator CutsceneTimeout()
        {
            yield return new WaitForSeconds(IntroVideoDuration);
            StartGame();
            LobbyManager.Singleton.PlaySoundClientRpc("PlayBossMusic");
        }

        private void StartGame()
        {
            isWatchingVideo = false;
            CutsceneOverlayUI.Singleton.HideCutsceneOverlayUI();
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    
        public void RenderPlayers(List<LobbyPlayer> playersToRender) 
        {
            GameUI.Singleton.playerList.Clear();
            bigPlayerName.text = Constants.NotSelected;
            wazoPlayerName.text = Constants.NotSelected;
            salumonPlayerName.text = Constants.NotSelected;
            foreach (LobbyPlayer player in playersToRender)
            {
                switch (player.GetPlayerClass())
                {
                    case PlayerClass.Mouse:
                        bigPlayerName.text = player.GetName().ToString();
                        break;
                    case PlayerClass.Bird:
                        wazoPlayerName.text = player.GetName().ToString();
                        break;
                    case PlayerClass.Fish:
                        salumonPlayerName.text = player.GetName().ToString();
                        break;
                    default:
                        Label playerName = new Label();
                        playerName.text = player.GetName().ToString();
                        playerName.style.color = new StyleColor(Color.white);
                        GameUI.Singleton.playerList.Add(playerName);
                        break;
                }
            }
        }
    
    }
}
