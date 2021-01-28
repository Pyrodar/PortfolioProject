using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretMenuButton : MonoBehaviour
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

    public void Initialize(TurretData td, int playerNumber)
    {
        HUD = (LoadoutHUD)MapLayoutInfo.Instance.HUD[playerNumber];
        data = td;
        Icon.sprite = data.Icon;
        turretName.text = data.name;

        switch (data.turretType)
        {
            case TurretType.AMS:
                Type.sprite = TypeAMS;
                break;
            case TurretType.AntiGround:
                Type.sprite = TypeATG;
                break;
            case TurretType.Missiles:
                Type.sprite = TypeMSL;
                break;
            default:
                break;
        }
    }

    public void TurretButtonClicked()
    {
        Debug.Log($"Clicked on button for turret: {data.name}");
        HUD.SwitchTurret(data);
    }
}
