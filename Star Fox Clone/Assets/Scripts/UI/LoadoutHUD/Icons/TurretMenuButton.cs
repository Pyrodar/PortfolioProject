using ProtocFiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretMenuButton : MonoBehaviour, IManeuverableListEntry
{
    [SerializeField] Image Background;
    [SerializeField] Image Icon;
    [SerializeField] Image Type;
    [SerializeField] Text turretName;

    [SerializeField] Sprite TypeATG;
    [SerializeField] Sprite TypeAMS;
    [SerializeField] Sprite TypeMSL;

    TurretData data;
    LoadoutHUD HUD;

    public void Initialize(TurretData td, UIBaseClass myHud)
    {
        HUD = (LoadoutHUD)myHud;
        data = td;
        Icon.sprite = data.Icon;
        turretName.text = data.name;

        switch (data.turretType)
        {
            case TurretClass_P.Ams:
                Type.sprite = TypeAMS;
                break;
            case TurretClass_P.Atg:
                Type.sprite = TypeATG;
                break;
            case TurretClass_P.Msl:
                Type.sprite = TypeMSL;
                break;
            default:
                break;
        }
    }

    public void MarkEntry()
    {
        Background.color = Color.red;
    }

    public void UnmarkEntry()
    {
        Background.color = Color.white;
    }

    public void SelectEntry()
    {
        TurretButtonClicked();
        //UnmarkEntry();
        HUD.deselectModule();
    }

    public void DeselectEntry()
    {
        //HUD.deselectModule();
    }

    public void TurretButtonClicked()
    {
        Debug.Log($"Clicked on button for turret: {data.name}");
        HUD.SwitchTurret(data);
    }
}
