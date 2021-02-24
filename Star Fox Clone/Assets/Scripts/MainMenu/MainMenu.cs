using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject Main;
    [SerializeField] GameObject Levels;
    [SerializeField] GameObject Settings;
    [SerializeField] GameObject Return;

    public void SingleplayerButton()
    {
        Main.SetActive(false);
        Return.SetActive(true);
        Levels.SetActive(true);

        GameStateConnection.Instance.SetPlayerNumber(1);
    }

    public void CoopButton()
    {
        Return.SetActive(true);
        Main.SetActive(false);
        Levels.SetActive(true);

        GameStateConnection.Instance.SetPlayerNumber(2);
    }

    public void SettingsButton()
    {
        Return.SetActive(true);
        Debug.Log("Showing Settings");
        Settings.SetActive(true);
        Main.SetActive(false);
    }

    public void ReturnButton()
    {
        Return.SetActive(false);
        Main.SetActive(true);
        Settings.SetActive(false);
        Levels.SetActive(false);
    }

    public void ExitButton()
    {
        Debug.Log("Exit Game");
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
