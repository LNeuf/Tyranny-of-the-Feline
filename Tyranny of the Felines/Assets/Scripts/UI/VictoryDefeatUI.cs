using System.Collections;
using System.Collections.Generic;
using Matryoshka.Lobby;
using Matryoshka.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class VictoryDefeatUI : MonoBehaviour
{
    public static VictoryDefeatUI Singleton { get; private set; }

    private const string VictoryDefeat = "VictoryDefeat";
    private const string ConditionLabel = "ConditionLabel";
    private const string ExitToLobbySelectorButton = "ExitToLobbySelectorButton";

    public VisualElement victoryDefeat;

    public Button exitToLobbySelectorButton;
    public Label conditionLabel;

    private bool isShowing;
    void Start()
    {
        Singleton = this;
        isShowing = false;

        var root = GetComponent<UIDocument>().rootVisualElement;

        victoryDefeat = root.Q<VisualElement>(VictoryDefeat);

        exitToLobbySelectorButton = root.Q<Button>(ExitToLobbySelectorButton);
        conditionLabel = root.Q<Label>(ConditionLabel);

        exitToLobbySelectorButton.clicked += ExitToLobbySelector;
    }

    public bool IsShowing()
    {
        return isShowing;
    }

    public void ShowVictoryDefeatUI(string condition)
    {
        isShowing = true;
        conditionLabel.text = condition;
        victoryDefeat.style.display = DisplayStyle.Flex;
    }
    
    public void ExitToLobbySelector()
    {
        SoundManager.Singleton.PlayMenuClick();
        isShowing = false;
        victoryDefeat.style.display = DisplayStyle.None;
        GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.lobbySelector);
        SceneManager.LoadScene("MainMenu");
        SoundManager.Singleton.PlayMenuMusic();
        NetworkManager.Singleton.gameObject.GetComponent<LobbyStart>().Start();
    }

}
