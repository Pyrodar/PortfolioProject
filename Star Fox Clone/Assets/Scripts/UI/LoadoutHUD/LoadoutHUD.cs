using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadoutHUD : UIBaseClass
{
    private void Awake() { uiType = UIType.Loadout; }

    #region ListingTurrets

    LoadoutList loadoutList;
    [SerializeField] TurretMenuButton TurretListPrefabs;
    [SerializeField] Transform turretListParent;

    List<TurretMenuButton> turretButtons;

    private void Start()
    {
        loadoutList = Resources.Load("LoadoutList") as LoadoutList;
        turretButtons = new List<TurretMenuButton>();
        turretsListed = new CyclingLists();
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

        turretButtons.Clear();
    }

    void fillListAll()
    {
        Debug.Log("Listing All");

        fillList(TurretType.AMS);
        fillList(TurretType.ATG);
        fillList(TurretType.MSL);
    }

    void fillList(TurretType type)
    {
        Debug.Log($"Listing {type}");

        switch (type)
        {
            case TurretType.AMS:

                for (int i = 0; i < GameConnection.Instance.AMS_Unlocked; i++)
                {
                    #region find turret
                    if (i > loadoutList.AMS_Turrets.Count)
                    {
                        Debug.LogError($"Number of unlocked AMS turrets is greater than number of total turrets: {i}");
                        i = 0;
                    }
                    TurretData turret = loadoutList.AMS_Turrets[i];
                    #endregion

                    #region spawn button
                    TurretMenuButton tb = Instantiate(TurretListPrefabs);
                    tb.Initialize(turret, this);
                    tb.transform.SetParent(turretListParent);
                    turretButtons.Add(tb);

                    resetScale(tb);
                    #endregion
                }
                break;

            case TurretType.ATG:
                for (int i = 0; i < GameConnection.Instance.ATG_Unlocked; i++)
                {
                    #region find turret
                    if (i > loadoutList.ATG_Turrets.Count)
                    {
                        Debug.LogError($"Number of unlocked ATG turrets is greater than number of total turrets: {i}");
                        i = 0;
                    }
                    TurretData turret = loadoutList.ATG_Turrets[i];
                    #endregion

                    #region spawn button
                    TurretMenuButton tb = Instantiate(TurretListPrefabs);
                    tb.Initialize(turret, this);
                    tb.transform.SetParent(turretListParent);
                    turretButtons.Add(tb);

                    resetScale(tb);
                    #endregion
                }
                break;

            case TurretType.MSL:

                for (int i = 0; i < GameConnection.Instance.MSL_Unlocked; i++)
                {
                    #region find turret
                    if (i > loadoutList.MSL_Turrets.Count)
                    {
                        Debug.LogError($"Number of unlocked MSL turrets is greater than number of total turrets: {i}");
                        i = 0;
                    }
                    TurretData turret = loadoutList.MSL_Turrets[i];
                    #endregion

                    #region spawn button
                    TurretMenuButton tb = Instantiate(TurretListPrefabs);
                    tb.Initialize(turret, this);
                    tb.transform.SetParent(turretListParent);
                    turretButtons.Add(tb);

                    resetScale(tb);
                    #endregion
                }
                break;

            default:
                break;
        }

        turretsListed.ResetLists(turretButtons.ToList<IManeuverableListEntry>());
    }

    /// <summary>
    ///After assigning new parent RectTransform seems to get messed up
    ///Afterwards the rotation seems to be making trouble éverytime it's reloaded
    /// </summary>
    /// <param name="t"></param>
    void resetScale(TurretMenuButton t)
    {
        t.GetComponent<RectTransform>().localPosition = Vector3.zero;
        t.transform.localScale = Vector3.one;
        t.transform.rotation = Camera.main.transform.rotation;
    }

    #region public "show" funktions
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
        fillList(TurretType.ATG);
    }
    public void ShowTurretsMSL()
    {
        clearList();
        fillList(TurretType.MSL);
    }
    #endregion

    #endregion

    #region selectingTurret

    [SerializeField]Transform TurretsList;
    [SerializeField]TurretDetails CurrentTurret;

    List<TurretModule> modules;
    TurretModule selectedModule;

    CyclingLists modulesListed;     //List for Modules
    CyclingLists turretsListed;     //List for TurretSelection
    CyclingLists maneuveredList;    //Currently active List

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
                icon.transform.SetParent(modulesLayout);
                icon.transform.localScale = Vector3.one;
                icon.transform.localPosition = Vector3.zero;
                moduleIcons.Add(icon);
            }

            //Create cyclingList element for keyboard/gamepad controlls
            List<IManeuverableListEntry> a = modules.ToList<IManeuverableListEntry>();
            List<IManeuverableListEntry> b = moduleIcons.ToList<IManeuverableListEntry>();
            
            maneuveredList = new CyclingLists(a);
            maneuveredList.AddList(b);

            modulesListed = maneuveredList;
        }
    }   //Creating modulesListed object

    bool switchedModule;
    

    private void Update()
    {
        if (modules == null) return;


        #region mouseInputs
        if (Input.GetMouseButtonDown(0))
        {
            //Ignore UI Inputs and inputs outside this players screen
            if (EventSystem.current.IsPointerOverGameObject() || !isMouseInViewport(Input.mousePosition))
            {
                return;
            }

            Ray ray = MapLayoutInfo.Instance.Cameras[playerNumber].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                //Debug.Log($"Clicked on: {hit.collider.name}");
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
                if (Input.GetButtonDown("MenuEnter")) selectEntry();
                if (Input.GetButtonDown("MenuBack")) deselectModule();
                break;

            case 1:
                axisInput = Input.GetAxisRaw("Horizontal-2");
                if (Input.GetButtonDown("MenuEnter-2")) selectEntry();
                if (Input.GetButtonDown("MenuBack-2")) deselectModule();
                break;
        }
        #endregion

        #region cycle through turret list
        if (Mathf.Abs(axisInput) == 1 && modules != null)
        {
            if (switchedModule) return;
            maneuveredList.moveSteps(Mathf.FloorToInt(axisInput));
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

    public void selectEntry()
    {
        //ERROR: dont select modules list entry
        if(maneuveredList == modulesListed) selectEntry(modules[maneuveredList.Index]);
        else maneuveredList.selectCurrentEntry();
    }

    public void selectEntry(IManeuverableListEntry entry)
    {
        TurretModule turretModule = (TurretModule)entry;
        if (turretModule != null)
        {
            selectModule(turretModule);
            return;
        }

        TurretMenuButton button = (TurretMenuButton)entry;
        if(button != null)
        {
            button.SelectEntry();
        }
    }

    public void selectModule(TurretModule tm)
    {
        deselectModule();

        #region show TurretList and Descriptions
        TurretsList.gameObject.SetActive(true);
        CurrentTurret.gameObject.SetActive(true);

        CurrentTurret.SetDescription(tm.CurrentTurret);
        CurrentTurret.SetQuadrants(tm.GetComponent<TurretMount>());
        #endregion


        selectedModule = tm;

        for (int i = 0; i < modules.Count; i++)
        {
            if (modules[i] == tm)
            {
                maneuveredList.moveTo(i);
                maneuveredList.selectCurrentEntry();
                continue;
            }
        }

        #region Switch maneuvered List to available Turrets

        maneuveredList = turretsListed;
        maneuveredList.moveSteps(0);                        //Mark current entry
        Debug.Log($"Maneuvered List is now: turretsListed");

        #endregion
    }

    public void deselectModule()
    {
        #region Hide TurretList and Description
        TurretsList.gameObject.SetActive(false);
        CurrentTurret.gameObject.SetActive(false);
        #endregion

        #region Switch maneuvered List back to modules

        maneuveredList.deselectCurrentEntry();

        maneuveredList = modulesListed;
        Debug.Log($"Maneuvered List is now: modulesListed");

        #endregion

        if (selectedModule == null) return;

        maneuveredList.deselectCurrentEntry();

        selectedModule = null;

        Debug.Log($"Selected Module is now nothing");
    }

    public void MarkModule(TurretModule tm)
    {
        for (int i = 0; i < modules.Count; i++)
        {
            if(modules[i] == tm)
            {
                maneuveredList.moveTo(i);
                continue;
            }
        }
    }

    public void UnmarkModule(TurretModule tm)
    {
        maneuveredList.jumpOfList();
    }

    #endregion

    #region SwitchTurret
    
    public void SwitchTurret(TurretData data)
    {
        selectedModule.ChangeTurret(data);
        CurrentTurret.SetDescription(selectedModule.CurrentTurret);
    }

    #endregion
}
