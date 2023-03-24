using Matryoshka.Http;
using Unity.Netcode;
using UnityEngine.UIElements;

namespace Matryoshka.UI
{
    public class GameUI : NetworkBehaviour
    {
        private static GameUI _instance;
        public static GameUI Singleton => _instance;

        public VisualElement mainMenu { get; private set; }
        public VisualElement options { get; private set; }
        public VisualElement lobbySelector { get; private set; }
        public VisualElement lobby { get; private set; }
        public VisualElement characterSelect { get; private set; }
        public VisualElement controlsInfo { get; private set; }
        public VisualElement playerList { get; private set; }

        private VisualElement previousScreen;
        private VisualElement currentScreen;

        public void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
    
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            var root = GetComponent<UIDocument>().rootVisualElement;
        
            mainMenu = root.Q<VisualElement>(Constants.MainMenu);
            lobbySelector = root.Q<VisualElement>(Constants.LobbySelector);
            lobby = root.Q<VisualElement>(Constants.Lobby);
            options = root.Q<VisualElement>(Constants.Options);
            characterSelect = root.Q<VisualElement>(Constants.CharacterSelect);
            controlsInfo = root.Q<VisualElement>(Constants.ControlsInfo);
            playerList = root.Q<VisualElement>(Constants.PlayerList);

            previousScreen = mainMenu;
            currentScreen = mainMenu;

            HttpClient.Singleton.GetServers();
        }

        public void ChangeCurrentScreenTo(VisualElement newScreen)
        {
            previousScreen = currentScreen;
            if(currentScreen != null)
            {
                currentScreen.style.display = DisplayStyle.None;
            }
            currentScreen = newScreen;
            newScreen.style.display = DisplayStyle.Flex;
        }

        public void ChangeScreenToPrevious()
        {
            var tempScreen = currentScreen;
            
            if (previousScreen != null)
            {
                if (currentScreen != null)
                {
                    currentScreen.style.display = DisplayStyle.None;
                }
                currentScreen = previousScreen;
                currentScreen.style.display = DisplayStyle.Flex;
                previousScreen = tempScreen;
            }
            else
            {
                HideScreen();
                PauseMenuUI.Singleton.ShowPauseMenuUI();
            }
        }

        public void HideScreen()
        {
            previousScreen = null;
            if(currentScreen != null)
            {
                currentScreen.style.display = DisplayStyle.None;
            }
            currentScreen = null;
        }

    }
}
