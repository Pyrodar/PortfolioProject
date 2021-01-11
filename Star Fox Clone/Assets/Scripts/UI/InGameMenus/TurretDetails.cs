using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretDetails : MonoBehaviour
{
    [SerializeField] Image Icon;
    [SerializeField] Image Type;

    [SerializeField] Text turretName;
    [SerializeField] Text typeName;
    [SerializeField] Text ammoType;
    [SerializeField] Text firingSpeed;
    //Last stat can be either firing speed or missle capacity
    [SerializeField] Text LastStat;
    //General Description of the turret
    [SerializeField] Text Description;

    [SerializeField] Sprite IconATG;
    [SerializeField] Sprite IconAMS;
    [SerializeField] Sprite IconMSL;

    TurretData data;

    public void SetDescription(TurretData td)
    {
        data = td;
        if(data == null)
        {
            clearDescription();
            return;
        }

        #region Icons
        Icon.sprite = data.Icon;
        switch (data.turretType)
        {
            case TurretType.AMS:
                Type.sprite = IconAMS;
                break;
            case TurretType.AntiGround:
                Type.sprite = IconATG;
                break;
            case TurretType.Missiles:
                Type.sprite = IconMSL;
                break;
            default:
                break;
        }
        #endregion

        #region Text

        turretName.text = data.name;

        switch (data.turretType)
        {
            case TurretType.Missiles:
                LastStat.text = "Missle Capacity";
                firingSpeed.text = data.missleSpace.ToString();
                ammoType.text = data.missleData.name;
                break;
            default:
                LastStat.text = "Firing Speed";
                firingSpeed.text = data.turretSpeed.ToString();
                ammoType.text = data.bulletData.name;
                break;
        }

        Description.text = data.Description;
        #endregion

    }

    void clearDescription()
    {
        Icon.sprite = null;
        Type.sprite = null;

        turretName.text = "/na";
        ammoType.text = "/na";

        LastStat.text = "Firing Speed";
        firingSpeed.text = "/na";

        Description.text = "/na";
    }

}
