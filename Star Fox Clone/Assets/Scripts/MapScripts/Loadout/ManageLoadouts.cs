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
    List<ShipSaveFile_S> saveFiles { get { return GameConnection.Instance.GameInfo.currentSaveFile.shipFiles; } }

    LoadoutList loadoutList;

    public void LoadRessources()
    {
        loadoutList = Resources.Load("LoadoutList") as LoadoutList;
    }
    #region save

    public void SaveCurrentLoadout(int file, int shipchassis, List<TurretModule> turretModules)
    {

        ShipSaveFile_S saveFile = new ShipSaveFile_S(shipchassis, new List<TurretMount_S>());

        foreach (var module in turretModules)
        {
            saveFile.TurretMounts.Add(getTurret(module));
        }

        saveFiles.RemoveAt(file);
        saveFiles.Insert(file, saveFile);

        Debug.LogWarning($"SaveFile has been overwritten with current Loadout");
        GameConnection.Instance.Save();
    }

    TurretMount_S getTurret(TurretModule module)
    {
        TurretClass_P type;
        int number;
        loadoutList.GetClassIndexFromTurretData(module.CurrentTurret, out type, out number);

        TurretMount_S turretMount = new TurretMount_S();

        turretMount.TurretType = type;
        turretMount.TurretIndex = number;

        return turretMount;
    }
    #endregion

    #region load
    public void loadLoadoutFromFile(int file, int playerNumber, List<TurretModule> turretModules)
    {
        if (saveFiles[file].TurretMounts.Count < 2)
        {
            Debug.LogWarning($"The file number {file} you are trying to load appears to be empty");
            return;
        }

        ChangeShip(file, playerNumber, saveFiles[file].ShipChassisIndex);

        //TODO: turretModules might have changed here

        changeAllTurretsFromFile(file, turretModules);
    }


    public void changeTurret(TurretModule module, TurretData data)
    {
        module.ChangeTurret(data);
    }


    void ChangeShip(int file, int playerNumber, int shiptype)
    {
        //TODO: change ship
    }

    void changeAllTurretsFromFile(int file, List<TurretModule> turretModules)
    {
        //checking if there are as many Modules on the ship as are mentioned in the savefile.
        if (turretModules.Count == saveFiles[file].TurretMounts.Count)
        {
            TurretMount_S[] turrets = new TurretMount_S[saveFiles[file].TurretMounts.Count];
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
    #endregion
}
