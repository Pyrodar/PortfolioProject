using ProtocFiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used to save and load different loadouts in the games savefile
/// in between the Loadout HUD and the TurretModule
/// </summary>
public class ManageLoadouts
{
    //Currently directly edits the saveFile, maybe later embed this funkktionality into the GameInfo file
    ShipSaveFile_P[] saveFiles { get { return GameConnection.Instance.GameInfo.currentSaveFile.shipFiles; } }

    LoadoutList loadoutList;

    public void LoadRessources()
    {
        loadoutList = Resources.Load("LoadoutList") as LoadoutList;
    }

    #region save/load complete file
    public void loadLoadoutFromFile(int file, int playerNumber, List<TurretModule> turretModules)
    {
        ChangeShip(file, playerNumber, saveFiles[file].ShipChassisIndex);

        //TODO: turretModules might have changed here

        changeAllTurretsFromFile(file, turretModules);
    }

    public void SaveCurrentLoadout(int file, int shipchassis, List<TurretModule> turretModules)
    {
        Debug.Log(saveFiles[file].TurretMounts.Count);

        saveFiles[file].ShipChassisIndex = shipchassis;

        //Set the required amount of entries in the ship save file 
        saveFiles[file].TurretMounts.Clear();
        saveFiles[file].TurretMounts.AddRange(new List<TurretMount_P>(turretModules.Count));


        foreach (var module in turretModules)
        {
            saveTurret(file, module, module.CurrentTurret);
        }

        Debug.LogWarning($"SaveFile has been overwritten with current Loadout");
        GameConnection.Instance.Save();
    }

    public void SaveCurrentLoadout(int file, List<TurretModule> turretModules)
    {
        SaveCurrentLoadout(file, 0, turretModules);
    }
    #endregion



    void ChangeShip(int file, int playerNumber, int shiptype)
    {
        //TODO: change saveFile

        //TODO: change ship
    }

    public void changeTurret(int file, TurretModule module, TurretData data)
    {
        saveTurret(file, module, data);

        module.ChangeTurret(data);
    }

    void saveTurret(int file, TurretModule module, TurretData data)
    {
        TurretClass_P type;
        int number;
        loadoutList.GetClassIndexFromTurretData(data, out type, out number);

        saveFiles[file].TurretMounts[module.ModuleNumber].TurretType =  type;
        saveFiles[file].TurretMounts[module.ModuleNumber].TurretIndex =  number;
    }

    void changeAllTurretsFromFile(int file, List<TurretModule> turretModules)
    {
        //checking if there are as many Modules on the ship as are mentioned in the savefile.
        if (turretModules.Count == saveFiles[file].TurretMounts.Count)
        {
            TurretMount_P[] turrets = new TurretMount_P[saveFiles[file].TurretMounts.Count];
            saveFiles[file].TurretMounts.CopyTo(turrets, 0);

            for (int i = 0; i < turretModules.Count; i++)
            {
                TurretData data = loadoutList.GetTurretDataFromInt((int)turrets[i].TurretType, turrets[i].TurretIndex);
                turretModules[i].ChangeTurret(data);
            }
        }
        else   //faulty savefile, is overwritten here
        {
            Debug.Log($"Current Modules: {turretModules.Count}");
            Debug.Log($"SaveFile Modules: {saveFiles[file].TurretMounts.Count}");
            Debug.LogError($"SaveFile has not the same amount of modules as the current ship!");

            //SaveCurrentLoadout(file, turretModules);
        }
    }
}
