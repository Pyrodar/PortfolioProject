using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretIconList : MonoBehaviour
{
    [SerializeField] TurretIcon turretIconPrefab;

    [SerializeField] Sprite AMSTurrretIcon;
    [SerializeField] Sprite AtGTurrretIcon;
    [SerializeField] Sprite MSSTurrretIcon;

    //List<TurretIcon> TurretIcons = new List<TurretIcon>();

    public void AddTurretToList(TurretMount mount)
    {
        TurretIcon icon = Instantiate(turretIconPrefab);
        Sprite sprite = generateSprite(mount.MyTurretType);

        icon.Initiate(mount, sprite);
        icon.transform.SetParent(transform);
    }

    Sprite generateSprite(TurretType type)
    {
        Sprite sprite;

        switch (type)
        {
            case TurretType.AMS:
                sprite = AMSTurrretIcon;
                break;
            case TurretType.AntiGround:
                sprite = AtGTurrretIcon;
                break;
            case TurretType.Missiles:
                sprite = MSSTurrretIcon;
                break;
            default:
                sprite = AMSTurrretIcon;
                Debug.LogError("error when generating Sprite for TurretIcon ");
                break;
        }

        return sprite;
    }
}
