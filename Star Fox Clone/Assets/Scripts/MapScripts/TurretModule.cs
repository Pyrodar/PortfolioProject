﻿using UnityEngine;
[RequireComponent(typeof(TurretMount))]
public class TurretModule : MonoBehaviour
{
    //Empty sphere Object to mark where to click
    //[SerializeField] GameObject clickableAreaMarker;
    GameObject areaMarker;
    SphereCollider coll;

    LoadoutHUD HUD;
    TurretData currentTurret;
    public TurretData CurrentTurret { get { return currentTurret; } }

    Material RegularAM;
    Material HoveredAM;
    Material SelectedAM;

    public void Instantiate()
    {
        //TODO: get 
        HUD = (LoadoutHUD)MapLayoutInfo.Instance.HUD[0];

        //Materials
        RegularAM = Resources.Load("RegularAM") as Material;
        HoveredAM = Resources.Load("HoveredAM") as Material;
        SelectedAM = Resources.Load("SelectedAM") as Material;

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
        //Debug.Log($"Klicked on: {name}");
        //Opening Turret Menu
        HUD.selectModule(this);
    }

    private void OnMouseEnter()
    {
        if (selected) return;

        //Debug.Log($"Hovering on: {name}");
        areaMarker.GetComponent<MeshRenderer>().material = HoveredAM;
    }

    private void OnMouseExit()
    {
        if (selected) return;

        //Debug.Log($"No longer hovering on: {name}");
        areaMarker.GetComponent<MeshRenderer>().material = RegularAM;
    }

    public void SelectModule()
    {
        Debug.Log($"Selected turret: {name}");
        selected = true;
        areaMarker.GetComponent<MeshRenderer>().material = SelectedAM;

    }

    public void DeselectModule()
    {
        Debug.Log($"Deselected turret: {name}");
        selected = false;
        areaMarker.GetComponent<MeshRenderer>().material = RegularAM;
    }

    #endregion

    /// <summary>
    /// creates the Turret as child of this transform based on a scriptableObject of type TurretData given to it
    /// </summary>
    /// <param name="turret">Is a reference to the turretType via a TurretData script that is used to create the new turret </param>
    public void AddTurret(TurretData turret)
    {
        #region set Mesh
        clearTurret();
        
        GameObject T = Instantiate(turret.TurretMesh);
        T.transform.parent = transform;
        T.transform.position = transform.position;
        T.transform.rotation = transform.rotation;
        Destroy(T.GetComponent<Turret>());
        #endregion

        #region set Script
        Turret TurretScript;
        
        switch (turret.turretType)
        {
            case TurretType.AMS:
                TurretScript = T.AddComponent<AMSTurret>();
                TurretScript.Data = turret;
                break;
            case TurretType.AntiGround:
                TurretScript = T.AddComponent<AntiGroundTurret>();
                TurretScript.Data = turret;
                break;
            case TurretType.Missiles:
                TurretScript = T.AddComponent<MissleTurret>();
                TurretScript.Data = turret;
                break;
        }
        #endregion

        refreshTurret();
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
}
