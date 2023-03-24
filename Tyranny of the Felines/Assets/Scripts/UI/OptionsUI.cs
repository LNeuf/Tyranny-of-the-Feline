using UnityEngine;
using UnityEngine.UIElements;

namespace Matryoshka.UI
{
    public class OptionsUI : MonoBehaviour
    {
        public static OptionsUI Singleton { get; private set; }
        private Button optionsBackButton;
        public SliderInt SFXVolumeSlider { get; private set; }
        public SliderInt MusicVolumeSlider { get; private set; }

        private bool isShowing;
        void Start()
        {
            Singleton = this;
        
            var root = GetComponent<UIDocument>().rootVisualElement;
        
            optionsBackButton = root.Q<Button>(Constants.OptionsBackButton);
            SFXVolumeSlider = root.Q<SliderInt>("SFXVolumeSlider");
            MusicVolumeSlider = root.Q<SliderInt>("MusicVolumeSlider");
        
            optionsBackButton.clicked += OptionsBackButtonPressed;
        }

        void OptionsBackButtonPressed()
        {
            SoundManager.Singleton.PlayMenuClick();
            PauseMenuUI.Singleton.LeftOptions();
            GameUI.Singleton.ChangeScreenToPrevious();
        }
    
    }
}
