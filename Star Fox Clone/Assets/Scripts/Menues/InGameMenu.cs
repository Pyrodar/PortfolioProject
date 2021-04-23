using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameObject Main;
    [SerializeField] GameObject MainButtons;
    [SerializeField] GameObject Settings;
    [SerializeField] GameObject Return;

    public void ToggleMenu()
    {
        Main.SetActive(!Main.activeSelf);
    }

    public void SettingsButton()
    {
        MainButtons.SetActive(false);

        Settings.SetActive(true);
        Return.SetActive(true);
    }

    public void ReturnButton()
    {
        Return.SetActive(false);
        Settings.SetActive(false);

        MainButtons.SetActive(true);
    }

    public void ExitButton()
    {
        GameConnection.Instance.ReturnToMenu();
    }
}
