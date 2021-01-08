using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    #region HUD
    [SerializeField] VerticalBar healthbar;
    public VerticalBar Healthbar
    {
        get { return healthbar; }
    }


    [SerializeField] TurretIconList turretIconList;
    public TurretIconList TurretIconList
    {
        get { return turretIconList; }
    }

    [SerializeField] Transform[] targetMarkers;
    public Transform[] TargetMarkers
    {
        get { return targetMarkers; }
    }

    #endregion


    #region LoadoutMenu
    [SerializeField] TurretMenu turretMenu;
    public TurretMenu TurretMenu
    {
        get { return turretMenu; }
    }

    #endregion
}
