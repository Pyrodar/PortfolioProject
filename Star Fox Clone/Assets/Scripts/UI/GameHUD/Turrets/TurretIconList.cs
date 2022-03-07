using ProtocFiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretIconList : MonoBehaviour
{
    [SerializeField] TurretIcon turretIconPrefab;

    [SerializeField] Sprite AMSTurrretIcon;
    [SerializeField] Sprite AtGTurrretIcon;
    [SerializeField] Sprite MSLTurrretIcon;
    [SerializeField] Sprite OTHTurrretIcon;

    //List<TurretIcon> TurretIcons = new List<TurretIcon>();

    public void AddTurretToList(TurretMount mount)
    {
        TurretIcon icon = Instantiate(turretIconPrefab);
        Sprite sprite = generateSprite(mount.MyTurretType);

        icon.Initiate(mount, sprite);
        icon.transform.SetParent(transform);
        resetScale(icon);
    }

    //After assigning new parent RectTransform seems to get messed up
    void resetScale(TurretIcon t)
    {
        t.GetComponent<RectTransform>().localPosition = Vector3.zero;
        t.transform.localScale = Vector3.one;
    }

    Sprite generateSprite(TurretClass_P type)
    {
        Sprite sprite;

        switch (type)
        {
            case TurretClass_P.Ams:
                sprite = AMSTurrretIcon;
                break;
            case TurretClass_P.Atg:
                sprite = AtGTurrretIcon;
                break;
            case TurretClass_P.Msl:
                sprite = MSLTurrretIcon;
                break;
            case TurretClass_P.Other:
                sprite = OTHTurrretIcon;
                break;
            default:
                sprite = AMSTurrretIcon;
                Debug.LogError("error when generating Sprite for TurretIcon ");
                break;
        }

        return sprite;
    }
}
