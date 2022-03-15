using ProtocFiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadoutList", menuName = "Custom SO / LoadoutList")]
public class LoadoutList : ScriptableObject
{
    public List<Player> Prefabs;
    public List<TurretData> AMS_Turrets;
    public List<TurretData> ATG_Turrets;
    public List<TurretData> MSL_Turrets;
    public List<TurretData> OTH_Turrets;



    #region Player
    public Player GetShipFromInt(int i)
    {
        Player data;

        data = Prefabs[i];

        return data;
    }

    public int GetIntFromShip(Player player)
    {
        int shipNumber = -1;

        for (int i = 0; i < Prefabs.Count; i++)
        {
            if (Prefabs[i].ShipData == player.ShipData)
            {
                shipNumber = i;
            }
        }

        if (shipNumber < 0)
        {
            shipNumber = 0;
            throw new System.ArgumentOutOfRangeException(shipNumber.ToString(), "there is no ship with that number in the loadout list");
        }

        return shipNumber;

    }
    #endregion

    #region Turrets
    public TurretData GetTurretDataFromInt(int type, int number)
    {
        TurretData data;

        switch (type)
        {
            case 0:
                data = AMS_Turrets[number];
                break;
            case 1:
                data = ATG_Turrets[number];
                break;
            case 2:
                data = MSL_Turrets[number];
                break;


            default:
                Debug.LogError($"{type} is not a valid input for 'GetTurretDataFromInt'");
                data = null;
                break;
        }

        return data;
    }

    public void GetClassIndexFromTurretData(TurretData data, out TurretClass_P type, out int number)
    {
        type = 0;
        number = 0;

        Debug.Log(data);

        switch (data.turretType)
        {
            case TurretClass_P.Ams:
                type = TurretClass_P.Ams;
                for (int i = 0; i < AMS_Turrets.Count; i++)
                {
                    if (data == AMS_Turrets[i])
                    {
                        number = i;
                        break;
                    }
                }
                break;

            case TurretClass_P.Atg:
                type = TurretClass_P.Atg;
                for (int i = 0; i < ATG_Turrets.Count; i++)
                {
                    if (data == ATG_Turrets[i])
                    {
                        number = i;
                        break;
                    }
                }
                break;

            case TurretClass_P.Msl:
                type = TurretClass_P.Msl;
                for (int i = 0; i < MSL_Turrets.Count; i++)
                {
                    if (data == MSL_Turrets[i])
                    {
                        number = i;
                        break;
                    }
                }
                break;

            case TurretClass_P.Other:
                type = TurretClass_P.Other;
                for (int i = 0; i < OTH_Turrets.Count; i++)
                {
                    if (data == OTH_Turrets[i])
                    {
                        number = i;
                        continue;
                    }
                }
                break;


            default:
                Debug.LogError($"{data} is not a valid input for 'GetIntFromTurretData'");
                break;
        }
    }
    #endregion
}
