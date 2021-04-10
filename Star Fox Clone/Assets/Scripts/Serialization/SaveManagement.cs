using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class SaveManagement
{
    #region BinaryFormatter
    public static void Save(object objectToSave, string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream($"Assets/SaveData/{fileName}.sav", FileMode.Create);
        bf.Serialize(stream, objectToSave);
        stream.Close();
    }

    public static object Load(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            FileStream stream = new FileStream($"Assets/SaveData/{fileName}.sav", FileMode.Open);
            object result = bf.Deserialize(stream);
            stream.Close();
            return result;
        }
        catch (System.Exception)
        {
            Debug.LogError($"Couldn't load file {fileName}.sav");
            return null;
        }

    }
    #endregion

    #region CreateSaveFiles
    public void SaveGame(string fileName, GameInfo state)
    {
        Debug.Log("saved: " + state.currentSaveFile);
        SaveFile file = state.currentSaveFile;
        Save(file, fileName);
    }

    public SaveFile LoadGameSave(string fileName)
    {
        object file = Load(fileName);
        if (file as SaveFile == null)
        {
            return null;
        }

        DebugSaveFile(file as SaveFile);
        return file as SaveFile;
    }

    public void DebugSaveFile(SaveFile file)
    {
        Debug.Log($"Levels Unlocked: {file.LevelsUnlocked}");
        Debug.Log($"AMS Unlocked: {file.AMSTurretsUnlocked}");
        Debug.Log($"ATG Unlocked: {file.ATGTurretsUnlocked}");
        Debug.Log($"MSL Unlocked: {file.MSLTurretsUnlocked}");

        foreach (var ship in file.shipFiles)
        {
            Debug.Log($"Current Ship: {ship.ShipData}");

            Debug.Log($"Current Turrettypes:");

            foreach (var turret in ship.TurretDatas)
            {
                Debug.Log(turret);
            }
        }
    }
    #endregion
}


