using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo
{
    #region Player
    string playerName;
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

    public ShipSaveFile[] shipFiles;

    /// <summary>
    /// creating default SaveFile
    /// </summary>
    public SaveFile()
    {
        LevelsUnlocked = 1;
        AMSTurretsUnlocked = 3;
        ATGTurretsUnlocked = 3;
        MSLTurretsUnlocked = 3;

        shipFiles = new ShipSaveFile[2];
        shipFiles[0] = new ShipSaveFile(0, new int[,] { { 0, 0 }, { 0, 0 }, { 1, 0 }, { 1, 0 }, { 2, 0 }, { 2, 0 } });
        shipFiles[1] = new ShipSaveFile(0, new int[,] { { 0, 0 }, { 0, 0 }, { 1, 0 }, { 1, 0 }, { 2, 0 }, { 2, 0 } });
    }

    //TODO: add funktions to unlock stuff 
}

/// <summary>
/// saving ship loadouts.
/// edited directly in the ManageLoadouts script
/// </summary>
[System.Serializable]
public struct ShipSaveFile
{
    public int ShipData;

    public int[,] TurretDatas;

    public ShipSaveFile(int ship, int[,] turrets)
    {
        ShipData = ship;
        TurretDatas = turrets;
    }
}

public enum ConnectionType
{
    SinglePlayer,
    LocalCoop,
    Host,
    Client
}
