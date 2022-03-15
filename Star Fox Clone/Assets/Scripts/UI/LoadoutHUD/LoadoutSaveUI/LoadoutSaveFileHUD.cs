using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadoutSaveFileHUD : MonoBehaviour
{
    [SerializeField]Transform MenuObject;
    [SerializeField]Transform ListObject;
    [SerializeField]List<LoadoutSaveFileHUDSlot> slots;
    LoadoutHUD HUD;
    SaveFile saveFile;

    public void Initialize(LoadoutHUD hud)
    {
        try
        {
            HUD = hud;
            saveFile = GameConnection.Instance.GameInfo.currentSaveFile;

            ShowSavedLoadouts();
        }
        catch (System.Exception)
        {
            Debug.LogError("Can't initialize LoadoutSaveFileMenu: ");// + e.ToString());
            throw;
        }

        //Hide Menu
        if(MenuObject.gameObject.activeSelf) ToggleMenu();
    }

    public void ShowSavedLoadouts()
    {
        int i = 0;

        Debug.Log("Initializing SaveSlots!");

        foreach (var slot in ListObject.GetComponentsInChildren<LoadoutSaveFileHUDSlot>())
        {
            slots.Add(slot);
            slot.Initialize(i);

            //Connect buttons with HUD
            slot.LoadFile += HUD.LoadSaveFile;
            slot.SaveFile += HUD.SaveCurrentLoadout;

            //Show current save file
            slot.ClearLoadout();
            if (saveFile.shipFiles.Count > i)
            {
                slot.SetLoadout(saveFile.shipFiles[i]);
            }

            i++;
        }
    }

    public void ToggleMenu()
    {
        MenuObject.gameObject.SetActive(!MenuObject.gameObject.activeSelf);
    }
}
