using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject Main;
    [SerializeField] GameObject Multiplayer;
    [SerializeField] GameObject MainButtons;
    [SerializeField] GameObject CoopButtons;
    [SerializeField] GameObject Levels;
    [SerializeField] GameObject Settings;
    [SerializeField] GameObject Return;

    public void CoopButton()
    {
        MainButtons.SetActive(false);
        CoopButtons.SetActive(true);
        Return.SetActive(true);
    }
    
    public void SingleplayerButton()
    {
        Main.SetActive(false);
        Return.SetActive(true);
        openLevelScreen();

        GameConnection.Instance.SetConnectionType(ConnectionType.SinglePlayer);
    }

    public void LocalCoopButton()
    {
        Return.SetActive(true);
        Main.SetActive(false);
        openLevelScreen();

        GameConnection.Instance.SetConnectionType(ConnectionType.LocalCoop);
    }

    public void HostCoopButton()
    {
        Return.SetActive(true);
        Main.SetActive(false);
        openLevelScreen();

        GameConnection.Instance.SetConnectionType(ConnectionType.Host);
    }

    public void JoinCoopButton()
    {
        Return.SetActive(true);
        Main.SetActive(false);
        openLevelScreen();

        GameConnection.Instance.SetConnectionType(ConnectionType.Client);
    }

    public void SettingsButton()
    {
        Return.SetActive(true);
        Settings.SetActive(true);
        Main.SetActive(false);
    }

    public void ReturnButton()
    {
        Return.SetActive(false);

        Main.SetActive(true);
        Settings.SetActive(false);
        Levels.SetActive(false);

        MainButtons.SetActive(true);
        CoopButtons.SetActive(false);
    }

    public void ExitButton()
    {
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void openLevelScreen()
    {
        Levels.SetActive(true);
        Levels.GetComponent<LevelSelect>().RefreshLevels();
    }





    #region Debugging
    public void LevelUnlock(int value)
    {
        GameConnection.Instance.LevelUnlock(value);
    }

    public void AMSUnlock(int value)
    {
        GameConnection.Instance.AMSUnlock(value);

    }
    public void ATGUnlock(int value)
    {
        GameConnection.Instance.ATGUnlock(value);

    }
    public void MSLUnlock(int value)
    {
        GameConnection.Instance.MSLUnlock(value);

    }
    #endregion
}
