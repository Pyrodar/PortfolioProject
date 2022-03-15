using ProtocFiles;
using UnityEngine;
[RequireComponent(typeof(TurretMount))]
public class TurretModule : MonoBehaviour, IManeuverableListEntry
{
    //Empty sphere Object to mark where to click
    GameObject areaMarker;
    SphereCollider coll;

    LoadoutHUD HUD;
    Player myPlayer;
    public int ModuleNumber;    //used to create a ship loadout save file, set in the Player script
    TurretData currentTurret;
    public TurretData CurrentTurret { get { return currentTurret; } }

    Material RegularAM;
    Material HoveredAM;
    Material SelectedAM;

    public void Instantiate(int i)
    {
        HUD = (LoadoutHUD)MapLayoutInfo.Instance.HUD[i];
        myPlayer = GameConnection.Instance.Players[i];

        //Materials
        RegularAM = Resources.Load("Materials/LoadoutHUD/RegularAM") as Material;
        HoveredAM = Resources.Load("Materials/LoadoutHUD/HoveredAM") as Material;
        SelectedAM = Resources.Load("Materials/LoadoutHUD/SelectedAM") as Material;

        //UI Markers
        areaMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        areaMarker.transform.position = transform.position;
        areaMarker.transform.localScale = new Vector3(0.65f,0.65f,0.65f);
        areaMarker.GetComponent<MeshRenderer>().material = RegularAM;

        //Making turret clickable
        areaMarker.gameObject.layer = 2;    //IgnoreRaycastLayer
        gameObject.layer = 0;               //DefaultLayer
        coll = gameObject.AddComponent<SphereCollider>();
        coll.radius = 0.325f;

        refreshTurret();
    }

    void refreshTurret()
    {
        currentTurret = gameObject.GetComponent<TurretMount>().GetTurretData();
    }

    #region MouseInteractions

    bool selected;

    private void OnMouseDown()
    {
        Debug.Log($"Klicked on: {name}");
        //Opening Turret Menu
        HUD.selectEntry(this);
    }

    private void OnMouseEnter()
    {
        if (selected) return;

        HUD.MarkModule(this);
    }

    private void OnMouseExit()
    {
        if (selected) return;

        //HUD.UnmarkModule(this);
    }
    #endregion

    #region changing turrets
    /// <summary>
    /// removes the old turret and adds the new one
    /// </summary>
    /// <param name="data">Is a reference to the turretType via a TurretData script that is used to create the new turret </param>
    public void ChangeTurret(TurretData data)
    {
        clearTurret();

        AddTurret(data);

        refreshTurret();
    }

    /// <summary>
    /// creates the Turret as child of this transform based on a scriptableObject of type TurretData given to it
    /// </summary>
    /// <param name="data"></param>
    void AddTurret(TurretData data)
    {
        #region set Mesh
        GameObject T = Instantiate(data.TurretMesh);
        T.transform.parent = transform;
        T.transform.position = transform.position;
        T.transform.rotation = transform.rotation;
        Destroy(T.GetComponent<Turret>());
        #endregion

        #region set Script
        Turret TurretScript;

        switch (data.turretType)
        {
            case TurretClass_P.Ams:
                TurretScript = T.AddComponent<AMSTurret>();
                TurretScript.Data = data;
                break;
            case TurretClass_P.Atg:
                TurretScript = T.AddComponent<ATGTurret>();
                TurretScript.Data = data;
                break;
            case TurretClass_P.Msl:
                TurretScript = T.AddComponent<MSLTurret>();
                TurretScript.Data = data;
                break;
        }
        #endregion
    }


    /// <summary>
    /// removes currently equipped Turret
    /// </summary>
    void clearTurret()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        refreshTurret();
    }
    #endregion

    #region start game
    /// <summary>
    /// removes marker and this script
    /// </summary>
    public void startGame()
    {
        Destroy(areaMarker);
        Destroy(coll);
        gameObject.layer = 2;       //IgnoreRaycastLayer
        Destroy(this);
    }
    #endregion

    #region Interface

    public void SelectEntry()
    {
        //Debug.Log($"Selected turret: {name}");
        selected = true;
        areaMarker.GetComponent<MeshRenderer>().material = SelectedAM;
    }

    public void DeselectEntry()
    {
        //Debug.Log($"Deselected turret: {name}");
        selected = false;
        areaMarker.GetComponent<MeshRenderer>().material = RegularAM;
    }

    public void MarkEntry()
    {
        areaMarker.GetComponent<MeshRenderer>().material = HoveredAM;
    }

    public void UnmarkEntry()
    {
        areaMarker.GetComponent<MeshRenderer>().material = RegularAM;
    }

    #endregion
}
