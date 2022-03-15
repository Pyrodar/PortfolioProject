using System.IO;
using UnityEngine;

public class SaveManagement
{
    #region Json
    public static void Save(SaveFile objectToSave, string fileName)
    {

        var json = JsonUtility.ToJson(objectToSave);

        Debug.Log("Shipfiles: " + objectToSave.shipFiles[0].ToString());
        Debug.Log(json);

        System.IO.File.WriteAllText($"Assets/SaveData/{fileName}.sav", json);
    }

    public static object Load(string fileName)
    {
        try
        {
            object result = JsonUtility.FromJson(File.ReadAllText($"Assets/SaveData/{fileName}.sav"),typeof(SaveFile));
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

        SaveFile file = state.currentSaveFile;
        if (file == null) return;

        Debug.Log("saving to File: " + fileName);
        
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
            Debug.Log($"Current Ship: {ship.ShipChassisIndex}");

            //Debug.Log($"Current Turrettypes:");

            foreach (var turret in ship.TurretMounts)
            {
                //Debug.Log(turret);
            }
        }
    }
    #endregion
}


