using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    tb.Initialize(turret, playerNumber);
                    tb.transform.SetParent(turretListParent);

                    resetScale(tb);
                }

                break;
            case TurretType.AntiGround:

                foreach (TurretData turret in availableTurretsATG)
                {
                    TurretMenuButton tb = Instantiate(TurretListPrefabs);
                    tb.Initialize(turret, playerNumber);
                    tb.transform.SetParent(turretListParent);

                    resetScale(tb);
                }

                break;
            case TurretType.Missiles:

                foreach (TurretData turret in availableTurretsMSL)
                {
                    TurretMenuButton tb = Instantiate(TurretListPrefabs);
                    tb.Initialize(turret, playerNumber);
                    tb.transform.SetParent(turretListParent);

                    resetScale(tb);
                }

                break;
            default:
                break;
        }
    }

    //After assigning new parent RectTransform seems to get messed up
    void resetScale(TurretMenuButton t)
    {
        t.GetComponent<RectTransform>().localPosition = Vector3.zero;
        t.transform.localScale = Vector3.one;
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

    List<TurretModule> modules;
    TurretModule selectedModule;

    CyclingLists cyclingLists;

    [SerializeField] ModuleListIcon ModuleIconPrefab;
    [SerializeField] Transform modulesLayout;
    List<ModuleListIcon> moduleIcons;
    public List<TurretModule> SetModuleList
    {
        set
        {
            modules = value;


            //list modules on the bottom of the screen
            moduleIcons = new List<ModuleListIcon>();
            foreach (var mod in modules)
            {
                ModuleListIcon icon = Instantiate(ModuleIconPrefab);
                icon.transform.parent = modulesLayout;
                icon.transform.localScale = Vector3.one;
                icon.transform.localPosition = Vector3.zero;
                moduleIcons.Add(icon);
            }

            //Create cyclingList element for keyboard/gamepad controlls
            List<IManeuverableListEntry> a = modules.ToList<IManeuverableListEntry>();
            List<IManeuverableListEntry> b = moduleIcons.ToList<IManeuverableListEntry>();
            
            cyclingLists = new CyclingLists(a);
            cyclingLists.AddList(b);

        }
    }
    bool switchedModule;
    

    private void Update()
    {
        if (modules == null) return;


        #region mouseInputs
        if (Input.GetMouseButtonDown(0))
        {
            //Ignore UI Inputs
            if (EventSystem.current.IsPointerOverGameObject() || !isMouseInViewport(Input.mousePosition))
            {
                return;
            }

            Ray ray = MapLayoutInfo.Instance.Cameras[playerNumber].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                Debug.Log($"Clicked on: {hit.collider.name}");
                //Deselect when no Turret is clicked on. Selecting right turret is handled in the TurretModule Script
                if (hit.collider.GetComponent<TurretModule>() == null)
                {
                    deselectModule();
                }
            }
            else
            {
                Debug.Log($"Clicked on: nothing");
                deselectModule();
            }
        }
        #endregion

        #region ModuleListInputs

        #region get input based on player
        float axisInput = 0;

        switch (playerNumber)
        {
            case 0:
                axisInput = Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("MenuEnter")) selectModule();
                if (Input.GetButtonDown("MenuBack")) deselectModule();
                break;

            case 1:
                axisInput = Input.GetAxisRaw("Horizontal-2");
                if (Input.GetButtonDown("MenuEnter-2")) selectModule();
                if (Input.GetButtonDown("MenuBack-2")) deselectModule();
                break;
        }
        #endregion

        #region cycle through turret list
        if (Mathf.Abs(axisInput) == 1 && modules != null)
        {
            if (switchedModule) return;
            cyclingLists.moveSteps(Mathf.FloorToInt(axisInput));
            switchedModule = true;
        }
        else switchedModule = false;
        #endregion

        #endregion
    }

    bool isMouseInViewport(Vector3 pos)
    {
        RectTransform rect = GetComponent<RectTransform>();

        //Vertical splitscreen
        if (Input.mousePosition.x < rect.sizeDelta.x * playerNumber) return false;
        if (Input.mousePosition.x > rect.sizeDelta.x + (rect.sizeDelta.x * playerNumber)) return false;

        //Horizontal splitscreen
        //if (Input.mousePosition.y < rect.sizeDelta.y * playerNumber) return false;
        //if (Input.mousePosition.y > rect.sizeDelta.y + (rect.sizeDelta.y * playerNumber)) return false;


        //Debug.Log("Mouse is in Screen " + playerNumber);
        return true;
    }

    public void selectModule(TurretModule tm)
    {
        deselectModule();

        #region show TurretList and Descriptions
        TurretsList.gameObject.SetActive(true);
        CurrentTurret.gameObject.SetActive(true);

        CurrentTurret.SetDescription(tm.CurrentTurret);
        CurrentTurret.SetQuadrants(tm.GetComponent<TurretMount>()); ;
        #endregion

        selectedModule = tm;

        for (int i = 0; i < modules.Count; i++)
        {
            if (modules[i] == tm)
            {
                cyclingLists.moveTo(i);
                cyclingLists.selectCurrentEntry();
                continue;
            }
        }
    }

    public void selectModule()
    {
        selectModule(modules[cyclingLists.Index]);
    }

    public void deselectModule()
    {
        #region Hide TurretList and Description
        TurretsList.gameObject.SetActive(false);
        CurrentTurret.gameObject.SetActive(false);
        #endregion

        if (selectedModule == null) return;

        cyclingLists.deselectCurrentEntry();

        selectedModule = null;
    }

    public void MarkModule(TurretModule tm)
    {
        for (int i = 0; i < modules.Count; i++)
        {
            if(modules[i] == tm)
            {
                cyclingLists.moveTo(i);
                continue;
            }
        }
    }

    public void UnmarkModule(TurretModule tm)
    {
        cyclingLists.jumpOfList();
    }

    #endregion

    #region SwitchTurret
    
    public void SwitchTurret(TurretData td)
    {
        selectedModule.AddTurret(td);
        CurrentTurret.SetDescription(selectedModule.CurrentTurret);
    }

    #endregion
}
