using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtocFiles;

public class GameInfo
{
    #region Player
    string playerName;

    const string versionNumber = "0.0";
    
    int myPlayer = 0;
    public int MyPlayer { get { return myPlayer; } }

    #region number of players
    int numberOfTotalPlayers;
    public int NumberOfTotalPlayers
    {
        get { return numberOfTotalPlayers; }
    }

    int numberOfLocalPlayers;
    public int NumberOfLocalPlayers
    {
        get { return numberOfLocalPlayers; }
    }
    #endregion

    #endregion

    #region ConnectionType

    ConnectionType connectionType = ConnectionType.Host;

    //public string NetworkAdress;

    public ConnectionType Connection
    {
        get { return connectionType; }
        set
        {
            switch (value)
            {
                case ConnectionType.SinglePlayer:
                    connectionType = value;
                    myPlayer = 0;
                    numberOfTotalPlayers = 1;
                    numberOfLocalPlayers = 1;
                    break;
                case ConnectionType.LocalCoop:
                    connectionType = value;
                    myPlayer = 0;
                    numberOfTotalPlayers = 2;
                    numberOfLocalPlayers = 2;
                    break;
                case ConnectionType.Host:
                    connectionType = value;
                    myPlayer = 0;
                    numberOfTotalPlayers = 2;
                    numberOfLocalPlayers = 1;
                    break;
                case ConnectionType.Client:
                    connectionType = value;
                    myPlayer = 1;
                    numberOfTotalPlayers = 2;
                    numberOfLocalPlayers = 1;
                    break;
            }
        }
    }

    #endregion

    #region saveFile
    public SaveFile currentSaveFile;

    public GameInfo(string filename)
    {
        LoadSaveFile(filename);
    }
    public void LoadSaveFile(string filename)
    {
        SaveManagement saveMan = new SaveManagement();
        SaveFile saveFile = saveMan.LoadGameSave(filename);

        if (saveFile == null)
        {
            //Debug.LogError($"Couldn't load File: {filename}");
            currentSaveFile = new SaveFile();
        }
        else
        {
            currentSaveFile = saveFile;
        }
    }
    #endregion
}

/// <summary>
/// The class that is used as the saveFile
/// </summary>
[System.Serializable]
public class SaveFile
{
    public int LevelsUnlocked;
    public int AMSTurretsUnlocked;
    public int ATGTurretsUnlocked;
    public int MSLTurretsUnlocked;

    public ShipSaveFile_P[] shipFiles;

    /// <summary>
    /// For creating default SaveFile
    /// unlocks the first three turrets of each type and saves 2 default ships.
    /// </summary>
    public SaveFile()
    {
        LevelsUnlocked = 1;
        AMSTurretsUnlocked = 3;
        ATGTurretsUnlocked = 3;
        MSLTurretsUnlocked = 3;

        shipFiles = new ShipSaveFile_P[2];
        shipFiles[0] = new ShipSaveFile_P() {   ShipChassisIndex = 0,
                                                TurretMounts = { 
                                                new TurretMount_P() { TurretType = TurretClass_P.Ams , TurretIndex = 0}, 
                                                new TurretMount_P() { TurretType = TurretClass_P.Ams, TurretIndex = 0 }, 
                                                new TurretMount_P() { TurretType = TurretClass_P.Atg, TurretIndex = 0 }, 
                                                new TurretMount_P() { TurretType = TurretClass_P.Atg, TurretIndex = 0 }, 
                                                new TurretMount_P() { TurretType = TurretClass_P.Msl, TurretIndex = 0 }, 
                                                new TurretMount_P() { TurretType = TurretClass_P.Msl, TurretIndex = 0 } }
        };

        shipFiles[1] = new ShipSaveFile_P() {   ShipChassisIndex = 0,
                                                TurretMounts = { 
                                                new TurretMount_P() { TurretType = TurretClass_P.Ams , TurretIndex = 0},
                                                new TurretMount_P() { TurretType = TurretClass_P.Ams, TurretIndex = 0 },
                                                new TurretMount_P() { TurretType = TurretClass_P.Atg, TurretIndex = 0 },
                                                new TurretMount_P() { TurretType = TurretClass_P.Atg, TurretIndex = 0 },
                                                new TurretMount_P() { TurretType = TurretClass_P.Msl, TurretIndex = 0 },
                                                new TurretMount_P() { TurretType = TurretClass_P.Msl, TurretIndex = 0 } }
        };
    }
    //TODO: add funktions to unlock stuff 
}

/// <summary>
/// DEPRECATED: now as Protoc file available
/// saving ship loadouts.
/// edited directly in the ManageLoadouts script
/// </summary>
//[System.Serializable]
//public struct ShipSaveFile
//{
//    public int ShipChassis;

//    public int[,] TurretDatas;

//    public ShipSaveFile(int ship, int[,] turrets)
//    {
//        ShipChassis = ship;
//        TurretDatas = turrets;
//    }
//}

public enum ConnectionType
{
    SinglePlayer,
    LocalCoop,
    Host,
    Client
}
