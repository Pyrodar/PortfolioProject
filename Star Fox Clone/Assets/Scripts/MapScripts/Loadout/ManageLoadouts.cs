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

    LoadoutList loadoutList = Resources.Load("LoadoutList") as LoadoutList;


    public void loadLoadoutFromFile(int playerNumber, int file, List<TurretModule> turretModules)
    {
        ChangeShip(file, playerNumber, saveFiles[file].ShipData);

        //TODO: turretModules might have changed here

        changeAllTurretsFromFile(file, turretModules);
    }

    void ChangeShip(int file, int playerNumber, int shiptype)
    {
        //TODO: change saveFile

        //TODO: change ship
    }



    public void changeTurret(int file, TurretModule module, TurretData data)
    {
        #region adjust saveFile
        int type;
        int number;
        loadoutList.GetIntFromTurretData(data, out type, out number);

        saveFiles[file].TurretDatas[module.ModuleNumber, 0] =  type;
        saveFiles[file].TurretDatas[module.ModuleNumber, 1] =  number;
        #endregion

        module.ChangeTurret(data);
    }

    void changeAllTurretsFromFile(int file, List<TurretModule> turretModules)
    {

        //TODO: check if the length of the turretModules is the same as the saveFiles turretlist

        int[,] turrets = saveFiles[file].TurretDatas;

        for (int i = 0; i < turretModules.Count; i++)
        {
            TurretData data = loadoutList.GetTurretDataFromInt(turrets[i, 0], turrets[i, 1]);
            turretModules[i].ChangeTurret(data);
        }
    }
}
