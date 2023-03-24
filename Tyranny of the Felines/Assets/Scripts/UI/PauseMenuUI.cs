using System;
using System.Collections;
using Matryoshka.Game;
using Matryoshka.Lobby;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Matryoshka.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        public static PauseMenuUI Singleton { get; private set; }
    
        private const string PauseMenu = "PauseMenu";
        private const string GoToOptionsButton = "PauseMenuGoToOptionsButton";
        private const string ExitToLobbySelectorButton = "PauseMenuExitToLobbySelectorButton";
        private const string BackToGameButton = "PauseMenuBackToGameButton";
        
        private const float Escape = 1f;
        private const float Released = 0f;

        public VisualElement pauseMenu;

        public Button goToOptionsButton;
        public Button exitToLobbySelectorButton;
        public Button backToGameButton;

        private bool isShowing;
        private bool isInOptions;
        private bool pressed;
        
        void Start()
        {
            Singleton = this;
            isShowing = false;
            pressed = false;
            var root = GetComponent<UIDocument>().rootVisualElement;
        
            pauseMenu = root.Q<VisualElement>(PauseMenu);

            goToOptionsButton = root.Q<Button>(GoToOptionsButton);
            exitToLobbySelectorButton = root.Q<Button>(ExitToLobbySelectorButton);
            backToGameButton = root.Q<Button>(BackToGameButton);

            goToOptionsButton.clicked += GoToOptions;
            exitToLobbySelectorButton.clicked += ExitToLobbySelector;
            backToGameButton.clicked += BackToGame;
        }

        public void LeftOptions()
        {
            isInOptions = false;
        }
    
        public bool IsShowing()
        {
            return isShowing || isInOptions;
        }

        private void FixedUpdate()
        {
            if (GameManager.Singleton != null && !GameManager.Singleton.postGame)
            {
                if (!pressed)
                {
                    if (Math.Abs(Input.GetAxis("Cancel") - Escape) < 0.0001)
                    {
                        if (isShowing)
                        {
                            if (!isInOptions)
                            {
                                pressed = true;
                                HidePauseMenuUI();
                            }
                        }
                        else
                        {
                            pressed = true;
                            ShowPauseMenuUI();
                        }
                    }
                }
                else
                {
                    if (Math.Abs(Input.GetAxis("Cancel") - Released) < 0.0001)
                    {
                        pressed = false;
                    }
                }
            }
        }

        public void ShowPauseMenuUI()
        {
            isShowing = true;
            pauseMenu.style.display = DisplayStyle.Flex;
        }

        public void HidePauseMenuUI()
        {
            isShowing = false;
            pauseMenu.style.display = DisplayStyle.None;
        }

        public void GoToOptions()
        {
            SoundManager.Singleton.PlayMenuClick();
            HidePauseMenuUI();
            isInOptions = true;
            GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.options);
        }

        public void ExitToLobbySelector()
        {
            SoundManager.Singleton.PlayMenuClick();
            HidePauseMenuUI();
            LobbyStart.leftLobby = true;
            if(NetworkManager.Singleton.IsServer)
            {
                LobbyManager.Singleton.GetComponent<NetworkObject>().Despawn();
            }
            NetworkManager.Singleton.Shutdown();
            GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.lobbySelector);
            SoundManager.Singleton.PlayMenuMusic();
            SceneManager.LoadScene("MainMenu");
        }

        public void BackToGame()
        {
            SoundManager.Singleton.PlayMenuClick();
            HidePauseMenuUI();
        }
    }
}
