using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadoutHUD : UIBaseClass
{
    private void Awake() { uiType = UIType.Loadout; }

    #region ListingTurrets

    [SerializeField] List<TurretData> availableTurretsATG;
    [SerializeField] List<TurretData> availableTurretsAMS;
    [SerializeField] List<TurretData> availableTurretsMSL;
    [SerializeField] TurretMenuButton TurretListPrefabs;
    [SerializeField] Transform turretListParent;

    private void Start()
    {
        clearList();
        fillListAll();
    }

    void clearList()
    {
        Debug.Log("clearing List");
        foreach (Transform child in turretListParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void fillListAll()
    {
        Debug.Log("Listing All");
        fillList(TurretType.AMS);
        fillList(TurretType.AntiGround);
        fillList(TurretType.Missiles);
    }

    void fillList(TurretType type)
    {
        Debug.Log($"Listing {type}");

        switch (type)
        {
            case TurretType.AMS:

                foreach (TurretData turret in availableTurretsAMS)
                {
                    TurretMenuButton tb = Instantiate(TurretListPrefabs);
                    tb.Initialize(turret);
                    tb.transform.SetParent(turretListParent);
                }

                break;
            case TurretType.AntiGround:

                foreach (TurretData turret in availableTurretsATG)
                {
                    TurretMenuButton tb = Instantiate(TurretListPrefabs);
                    tb.Initialize(turret);
                    tb.transform.SetParent(turretListParent);
                }

                break;
            case TurretType.Missiles:

                foreach (TurretData turret in availableTurretsMSL)
                {
                    TurretMenuButton tb = Instantiate(TurretListPrefabs);
                    tb.Initialize(turret);
                    tb.transform.SetParent(turretListParent);
                }

                break;
            default:
                break;
        }
    }

    public void ShowAllTurrets()
    {
        clearList();
        fillListAll();
    }
    public void ShowTurretsAMS()
    {
        clearList();
        fillList(TurretType.AMS);
    }
    public void ShowTurretsATG()
    {
        clearList();
        fillList(TurretType.AntiGround);
    }
    public void ShowTurretsMSL()
    {
        clearList();
        fillList(TurretType.Missiles);
    }

    #endregion


    #region selectingTurret

    [SerializeField]Transform TurretsList;
    [SerializeField]TurretDetails CurrentTurret;
    
    TurretModule selectedModule;

    private void Update()
    {
        //TODO: Set proper inputs
        if (Input.GetMouseButtonDown(0))
        {
            //Ignore UI Inputs
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

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
        TurretsList.gameObject.SetActive(true);
        CurrentTurret.gameObject.SetActive(true);

        CurrentTurret.SetDescription(tm.CurrentTurret);

        //Unmark Turret in UI
        if(selectedModule != null) selectedModule.DeselectModule();

        selectedModule = tm;

        //Mark Turret in UI
        selectedModule.SelectModule();
    }

    public void deselectModule()
    {
        TurretsList.gameObject.SetActive(false);
        CurrentTurret.gameObject.SetActive(false);

        if (selectedModule == null) return;

        //Unmark Turret in UI
        selectedModule.DeselectModule();


        selectedModule = null;
    }

    #endregion
}
