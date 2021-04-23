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
    ShipSaveFile[] saveFiles { get { return GameConnection.Instance.GameInfo.currentSaveFile.shipFiles; } }

    LoadoutList loadoutList;

    public void LoadRessources()
    {
        loadoutList = Resources.Load("LoadoutList") as LoadoutList;
    }

    #region save/load complete file
    public void loadLoadoutFromFile(int file, int playerNumber, List<TurretModule> turretModules)
    {
        ChangeShip(file, playerNumber, saveFiles[file].ShipData);

        //TODO: turretModules might have changed here

        changeAllTurretsFromFile(file, turretModules);
    }

    public void SaveCurrentLoadout(int file, List<TurretModule> turretModules)
    {
        Debug.Log(saveFiles[file].TurretDatas.Length);
        saveFiles[file].TurretDatas = new int[turretModules.Count, 2];

        foreach (var module in turretModules)
        {
            saveTurret(file, module, module.CurrentTurret);
        }

        Debug.LogWarning($"SaveFile has been overwritten with current Loadout");
        GameConnection.Instance.Save();
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
        int type;
        int number;
        loadoutList.GetIntFromTurretData(data, out type, out number);

        saveFiles[file].TurretDatas[module.ModuleNumber, 0] =  type;
        saveFiles[file].TurretDatas[module.ModuleNumber, 1] =  number;
    }

    void changeAllTurretsFromFile(int file, List<TurretModule> turretModules)
    {
        //checking if there are as many Modules on the ship as are mentioned in the savefile.
        //the safefile uses a 2 dimensional array, therefore its length is divided by 2
        if (turretModules.Count == saveFiles[file].TurretDatas.Length / 2)
        {
            int[,] turrets = saveFiles[file].TurretDatas;

            for (int i = 0; i < turretModules.Count; i++)
            {
                TurretData data = loadoutList.GetTurretDataFromInt(turrets[i, 0], turrets[i, 1]);
                turretModules[i].ChangeTurret(data);
            }
        }
        else   //faulty savefile, is overwritten here
        {
            Debug.Log($"Current Modules: {turretModules.Count}");
            Debug.Log($"SaveFile Modules: {saveFiles[file].TurretDatas.Length / 2}");
            Debug.LogError($"SaveFile has not the same amount of modules as the current ship!");

            SaveCurrentLoadout(file, turretModules);
        }
    }
}
