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
    /// creates the Turret as child of this transform based on a prefab given to it
    /// </summary>
    /// <param name="turret">Is a prefab containing the 3D object and the right Turret script (AMS, ATG or MSS)</param>
    public void AddTurret(GameObject turret)
    {
        clearTurret();
        GameObject T = Instantiate(turret);
        T.transform.parent = transform;
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
