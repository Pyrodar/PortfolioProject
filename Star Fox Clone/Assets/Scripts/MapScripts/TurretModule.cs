using UnityEngine;
[RequireComponent(typeof(TurretMount))]
public class TurretModule : MonoBehaviour
{
    //Empty sphere Object to mark where to click
    [SerializeField] GameObject clickableAreaMarker;
    GameObject AM;

    private void Awake()
    {
        AM = Instantiate(clickableAreaMarker);
        AM.transform.position = transform.position;
    }

    //Debugging
    private void Start()
    {
        startGame();
    }

    private void OnMouseDown()
    {
        Debug.Log("Klicked on: " + name);
        //Open Turret Menu
    }

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
        Destroy(AM);
        gameObject.layer = 2;
        Destroy(this);
    }
}
