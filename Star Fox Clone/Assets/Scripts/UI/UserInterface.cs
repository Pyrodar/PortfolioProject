using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    #region singleton
    static UserInterface instance;

    public static UserInterface Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (UserInterface.instance != null)
        {
            Debug.LogWarning("More than one instance of Player");
            return;
        }
        instance = this;
    }
    #endregion

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


    [SerializeField] TurretMenu turretMenu;
    public TurretMenu TurretMenu
    {
        get { return turretMenu; }
    }
}
