using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    private void Awake()
    {
        Settings.InitiailzeSettings();
    }

    public static void StartClient()
    {
        Settings.SetMultiplayerMode(MultiplayerMode.CLIENT);
        SceneManager.LoadScene(GlobalVariables.ClientHostSceneName);
    }

    public static void StartHost()
    {
        Settings.SetMultiplayerMode(MultiplayerMode.HOST);
        SceneManager.LoadScene(GlobalVariables.ClientHostSceneName);
    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }
}
