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
            throw new System.ArgumentOutOfRangeException(shipNumber.ToString(), "there is no ship with that number in the loadout list");
            shipNumber = 0;
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

    public void GetIntFromTurretData(TurretData data, out int type, out int number)
    {
        type = 0;
        number = 0;

        switch (data.turretType)
        {
            case TurretType.AMS:
                type = 0;
                for (int i = 0; i < AMS_Turrets.Count; i++)
                {
                    if (data == AMS_Turrets[i])
                    {
                        number = i;
                        continue;
                    }
                }
                break;

            case TurretType.ATG:
                type = 1;
                for (int i = 0; i < ATG_Turrets.Count; i++)
                {
                    if (data == ATG_Turrets[i])
                    {
                        number = i;
                        continue;
                    }
                }
                break;

            case TurretType.MSL:
                type = 2;
                for (int i = 0; i < MSL_Turrets.Count; i++)
                {
                    if (data == MSL_Turrets[i])
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
