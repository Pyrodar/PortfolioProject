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
    [SerializeField] Text SpeedAndCapacity;
    [SerializeField] Text AccuracyAndReloadtime;
    //Last two stat can be either firing speed or missle capacity
    [SerializeField] Text Stat4Name;
    [SerializeField] Text Stat5Name;
    //General Description of the turret
    [SerializeField] Text Description;

    //Quadrant Markers
    [SerializeField] Image[] Quadrants;

    //TurretType Icons
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
                typeName.text = "Anti Missle System";
                Type.sprite = IconAMS;
                break;
            case TurretType.ATG:
                typeName.text = "Air to Ground";
                Type.sprite = IconATG;
                break;
            case TurretType.MSL:
                typeName.text = "Missles";
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
            case TurretType.MSL:
                ammoType.text = data.missleData.name;
                
                Stat4Name.text = "Missle Capacity:";
                SpeedAndCapacity.text = data.missleSpace.ToString();
                
                Stat5Name.text = "Reload Time:";
                AccuracyAndReloadtime.text = $"{data.cooldown} sec";
                break;
            default:
                ammoType.text = data.bulletData.name;
                
                Stat4Name.text = "Firing Rate:";
                SpeedAndCapacity.text = $"{60 * (1 / data.cooldown)} rpm";

                Stat5Name.text = "Bullet Spread:";
                AccuracyAndReloadtime.text = data.bulletSpread.ToString();
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

        Stat4Name.text = "Firing Speed:";
        SpeedAndCapacity.text = "/na";

        Stat5Name.text = "Accuracy:";
        AccuracyAndReloadtime.text = "/na";

        Description.text = "/na";
    }


    public void SetQuadrants(TurretMount t)
    {
        foreach (Image q in Quadrants)
        {
            q.gameObject.SetActive(false);
        }

        foreach (int i in t.Quarters)
        {
            Quadrants[i].gameObject.SetActive(true);
        }
    }
}
