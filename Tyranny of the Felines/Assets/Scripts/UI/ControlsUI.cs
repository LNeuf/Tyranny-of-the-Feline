using System.Collections;
using System.Collections.Generic;
using Matryoshka.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlsUI : MonoBehaviour
{
    
    public const string ReturnToMainMenu = "ControlsReturnToMainMenuButton";

    public Button returnToMainMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        returnToMainMenuButton = root.Q<Button>(ReturnToMainMenu);

        returnToMainMenuButton.clicked += ReturnToMainMenuButtonPressed;
    }
    
    private void ReturnToMainMenuButtonPressed()
    {
        SoundManager.Singleton.PlayMenuClick();
        GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.mainMenu);
    }
}
