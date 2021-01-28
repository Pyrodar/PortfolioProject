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
    
    TurretModule selectedModule;

    private void Update()
    {
        //TODO: Set proper inputs
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
        TurretsList.gameObject.SetActive(true);
        CurrentTurret.gameObject.SetActive(true);

        CurrentTurret.SetDescription(tm.CurrentTurret);
        CurrentTurret.SetQuadrants(tm.GetComponent<TurretMount>()); ;

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

    #region SwitchTurret
    
    public void SwitchTurret(TurretData td)
    {
        selectedModule.AddTurret(td);
        CurrentTurret.SetDescription(selectedModule.CurrentTurret);
    }

    #endregion
}
