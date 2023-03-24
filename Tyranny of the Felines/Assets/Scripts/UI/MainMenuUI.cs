using Matryoshka.Http;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Matryoshka.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public static MainMenuUI Singleton { get; private set; }
        private Button playButton;
        private Button optionsButton;
        private Button quitButton;
        private Button creditsButton;
        private Button controlsButton;

        void Start()
        {
            Singleton = this;
        
            var root = GetComponent<UIDocument>().rootVisualElement;

            playButton = root.Q<Button>(Constants.PlayButton);
            optionsButton = root.Q<Button>(Constants.OptionsButton);
            quitButton = root.Q<Button>(Constants.QuitButton);
            creditsButton = root.Q<Button>(Constants.CreditsButton);
            controlsButton = root.Q<Button>(Constants.ControlsButton);
        
            playButton.clicked += PlayButtonPressed;
            quitButton.clicked += QuitButtonPressed;
            optionsButton.clicked += OptionsButtonPressed;
            creditsButton.clicked += CreditsButtonPressed;
            controlsButton.clicked += ControlsButtonClicked;
        }
    
        void PlayButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.lobbySelector);
            HttpClient.Singleton.GetServers();
        }
    
        void QuitButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            Application.Quit();
        }
    
        void OptionsButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.options);
        }
        
        void CreditsButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            GameUI.Singleton.HideScreen();
            SceneManager.LoadScene("Credits");
        }

        void ControlsButtonClicked()
        {
            SoundManager.Singleton.PlayMenuClick();
            GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.controlsInfo);
        }
    }
}
