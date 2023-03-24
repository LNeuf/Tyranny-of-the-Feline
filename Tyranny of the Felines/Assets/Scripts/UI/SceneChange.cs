using Matryoshka.UI;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Lobby;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SoundManager.Singleton.PlayMenuClick();
        GameUI.Singleton.ChangeCurrentScreenTo(GameUI.Singleton.mainMenu);
        SceneManager.LoadScene(sceneName);
    }
}
