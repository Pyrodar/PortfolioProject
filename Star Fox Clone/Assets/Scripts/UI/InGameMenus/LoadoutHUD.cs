using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutHUD : UIBaseClass
{
    private void Awake() { uiType = UIType.Loadout; }

    #region selectingTurret

    TurretModule selectedModule;

    private void Update()
    {
        //TODO: Set proper inputs
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                //Deselect when no Turret is clicked on. Selecting right turret is handled in the TurretModule Script
                if (hit.collider.GetComponent<TurretModule>() == null) 
                {
                    deselectModule();
                }
            }
            else
            {
                deselectModule();
            }
        }
    }

    public void selectModule(TurretModule tm)
    {
        //Unmark Turret in UI
        if(selectedModule != null) selectedModule.DeselectModule();

        selectedModule = tm;

        //Mark Turret in UI
        selectedModule.SelectModule();
    }

    public void deselectModule()
    {
        if (selectedModule == null) return;

        //Unmark Turret in UI
        selectedModule.DeselectModule();


        selectedModule = null;
    }

    #endregion


    #region TurretMenu

    [SerializeField] TurretMenu turretMenu;
    public TurretMenu TurretMenu
    {
        get { return turretMenu; }
    }

    #endregion
}
