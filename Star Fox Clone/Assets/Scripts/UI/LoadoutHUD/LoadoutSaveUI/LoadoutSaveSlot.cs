using ProtocFiles;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadoutSaveSlot : MonoBehaviour
{
    public UnityAction<int> LoadFile;
    public UnityAction<int> SaveFile;

    int slotNumber;

    [SerializeField]Text loadoutDescription;
    [SerializeField] Button LoadButton;
    [SerializeField] Button SaveButton;

    public void Initialize(int slotNo)
    {
        slotNumber = slotNo;
        LoadButton.onClick.AddListener(LoadThisFile);
        SaveButton.onClick.AddListener(SaveThisFile);
    }

    public void SetLoadout(ShipSaveFile_P saveData)
    {
        string description = "";
        
        foreach (var mount in saveData.TurretMounts)
        {
            description += Enum.GetName(typeof(TurretClass_P), mount.TurretType) + " " + mount.TurretIndex + " ";
        }

        loadoutDescription.text = description;
    }

    public void ClearLoadout()
    {
        loadoutDescription.text = "Slot Free";
    }

    void LoadThisFile()
    {
        LoadFile.Invoke(slotNumber);
    }

    void SaveThisFile()
    {
        SaveFile.Invoke(slotNumber);
    }


}
