using UnityEngine;
[RequireComponent(typeof(TurretMount))]
public class TurretModule : MonoBehaviour
{
    //Empty sphere Object to mark where to click
    //[SerializeField] GameObject clickableAreaMarker;
    GameObject areaMarker;
    SphereCollider coll;

    LoadoutHUD HUD;

    public void Instantiate()
    {
        //TODO: get 
        HUD = (LoadoutHUD)MapLayoutInfo.Instance.HUD[0];

        areaMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        areaMarker.transform.position = transform.position;
        areaMarker.GetComponent<MeshRenderer>().materials[0] = Resources.Load("KlickableAreaMarkerMat.mat") as Material;

        //Making turret clickable
        areaMarker.gameObject.layer = 2;    //IgnoreRaycastLayer
        gameObject.layer = 0;               //DefaultLayer
        coll = gameObject.AddComponent<SphereCollider>();
        coll.radius = 0.5f;
    }

    #region MouseInteractions

    private void OnMouseDown()
    {
        Debug.Log($"Klicked on: {name}");

        //Open Turret Menu
        HUD.selectModule(this);
    }

    private void OnMouseEnter()
    {
        Debug.Log($"Hovering on: {name}");
        //areaMarker.GetComponent<MeshRenderer>().materials[0] = ;
    }

    private void OnMouseExit()
    {
        Debug.Log($"No longer hovering on: {name}");
        //areaMarker.GetComponent<MeshRenderer>().materials[0] = ;
    }

    public void SelectModule()
    {
        Debug.Log($"Selected turret: {name}");
    }

    public void DeselectModule()
    {
        Debug.Log($"Deselected turret: {name}");
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
        
        GameObject T = Instantiate(turret.emptyTurretMesh);
        T.transform.parent = transform;
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
    }

    /// <summary>
    /// removes currently equipped Turret
    /// </summary>
    void clearTurret()
    {
        foreach (Transform child in transform)
        {
            Destroy(child);
        }
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
