using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour , IVehicle
{
    /// <summary>
    /// Everything needed for the game to work
    /// </summary>
    #region SetOnLevelLoaded
    [SerializeField]GameplayPlane plane;
    public GameplayPlane Plane
    {
        get { return plane; }
        set { plane = value; }
    }
    [SerializeField] Camera cam;
    public Camera Cam
    {
        set { cam = value; }
    }
    bool inGame;
    public bool IsInGame
    {
        get { return inGame; }
        set { inGame = value; }
    }
    InGameHUD hud;
    public InGameHUD HUD
    {
        set { hud = value; }
    }

    #endregion

    /// <summary>
    ///Required for the game to work and saved before starting the game in the prefab
    ///TODO: Visuals and HItbox must be interchangable based on ship type
    /// </summary>
    #region SetManuallyInEditor
    [SerializeField] TurretMount[] turretMounts;
    public TurretMount[] TurretMounts
    {
        get { return turretMounts; }
    }

    [SerializeField] Transform playerVisuals;
    [SerializeField] Transform playerRotationVisuals;
    [SerializeField] Transform shipFokusPoint;
    [SerializeField] Transform smallCrossHair;
    [SerializeField] Transform largeCrossHair;
    public Transform LargeCrosshair { get { return largeCrossHair; } }
    [SerializeField] GameObject crashExplosion;
    Rigidbody myRigid;
    #endregion

    /// <summary>
    /// All variables that affect the Gameplay feeling
    /// TODO: NEEDS TO BE INTERCHANGABLE BASED ON SHIPTYPE!!!
    /// </summary>
    #region Shiptype dependent

    #region Movement
    [SerializeField] float moveSpeed = 4.6f;
    [SerializeField] float rotationSpeed = 0.7f;

    [SerializeField] float fokusPointRange = 2f;
    [SerializeField] float fokusPointSpeed = 0.04f;
    [SerializeField] float fokusPointDamping = 2f; 
    //[SerializeField]
    [Tooltip("how fast the ship returns to looking straight forward again. fastest: 0.0 slowest: 1.0")]
    [Range(0.5f, 1f)]
    float fokusPointCenteringSpeed = 0.995f;
    [SerializeField]
    [Tooltip("how close to the ships front the crosshair returns after stopping the inputs")]
    float fokusPointCenteringTolerance = 0.01f;

    [SerializeField] float gravityAndLift = 0.4f;

    [SerializeField] Vector3 crosshairOffset;
    #endregion

    #region Combat
    [SerializeField] float maxHealth = 100;
    float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    [SerializeField] float invulnTime = 1;
    float invulnTimeEnd = 1;
    #endregion

    #endregion

    /// <summary>
    /// will be set automatically when the Game starts
    /// </summary>
    #region Targets and Turrets
    List<TurretMount> ATGTurrets = new List<TurretMount>();
    List<TurretMount> AMSTurrets = new List<TurretMount>();
    List<MissleTurret> MissleTurrets = new List<MissleTurret>();
    int lastTurret = 0; //Used to cycle MissleTurrets

    List<MissleData> misslesAvailable = new List<MissleData>();
    MissleData currentMissles;

    List<AquiredTarget> Targets = new List<AquiredTarget>();
    List<AquiredTarget> incomingMissles = new List<AquiredTarget>();
    private int maxTargets = 3;
    [SerializeField] Transform[] TargetMarker;
    #endregion


    #region HUD
    VerticalBar[] healthbars;
    #endregion

    #region InputFunktionBools
    bool DPadBool = true;
    #endregion 

    #region Debugging
    private void OnMouseDown()
    {
        Debug.Log("Klicked on: " + name);
    }
    #endregion


    /// <summary>
    /// Setting some values required for the game to funktion.
    /// currently too split between here and GameStateConnection
    /// </summary>
    public void StartGame()
    {
        #region HUD Setup
        currentHealth = maxHealth;
        healthbars = hud.Healthbars;

        foreach (VerticalBar item in healthbars)
        {
            item.Initialize(maxHealth);
        }


        TurretIconList list = hud.TurretIconList;


        for (int i = 0; i < 3; i++)
        {
            TargetMarker[i] = hud.TargetMarkers[i];
        }
        #endregion

        #region Rigidbody Setup

        Debug.Log("Adding RigidBody");
        //Rigidbody messes with the TurretColliders, therefore it's only added once the Game Starts
        myRigid = this.gameObject.AddComponent<Rigidbody>();
        myRigid.mass = 1;
        myRigid.drag = 2;
        myRigid.useGravity = false;
        myRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY| RigidbodyConstraints.FreezeRotationZ;

        #endregion

        #region Turrets Setup
        Debug.Log("Listing Turrets");


        foreach (var tur in turretMounts)
        {
            list.AddTurretToList(tur);
            if (tur.MyTurretType == TurretType.AntiGround) ATGTurrets.Add(tur);
            if (tur.MyTurretType == TurretType.AMS) AMSTurrets.Add(tur);
            if (tur.MyTurretType == TurretType.Missiles)
            {
                MissleTurrets.Add(tur.getMissleTurret());

                //Adding all available MissleTypes
                if (!misslesAvailable.Contains(tur.getMissleTurret().Data.missleData))
                {
                    misslesAvailable.Add(tur.getMissleTurret().Data.missleData);
                    currentMissles = misslesAvailable[0];
                    UpdateSelectedMissle();
                }
            }
        }
        #endregion

        inGame = true;
    }

    /// <summary>
    /// processing Inputs 
    /// TODO: move to Input Managing Script
    /// 
    /// Adjusting plane transform based on Fokuspoint position
    /// recalculates target locations and updates HUD accordingly
    /// applies gravity and lift
    /// </summary>
    void Update()
    {
        if (!inGame) return;
        //Movement
        //applyMovement(inputMovement());

        //checkBoundaries();

        //applyRotation(inputRotation());

        //clampFokusPointPosition();

        HelperFunctions.LookAt(playerVisuals, shipFokusPoint.position, fokusPointDamping);

        gravityAndLiftEffect();

        //Combat
        //combatInputs();

        applyTargetMarkers();

        designateTargets();

        designateMissles();
    }

    /// <summary>
    /// Contains the functions for movement and rotation of the plane
    /// </summary>
    /// <param name="input">Vector from the InputManager delivering the directional inputs</param>
    #region Movement

    public void ApplyMovement(Vector3 input)
    {
        myRigid.AddForce(input * moveSpeed);
        updateFokusPoint(input);

        checkBoundaries();
        clampFokusPointPosition();
    }

    void checkBoundaries()
    {
        //setting velocity to 0 when hitting boundries so enemy Missles and AA don't get confused
        float x = cam.transform.localPosition.x;
        if (transform.localPosition.x >= Plane.MaxWidth + x) myRigid.velocity = new Vector3(0, myRigid.velocity.y, myRigid.velocity.z);
        if (transform.localPosition.x <= -Plane.MaxWidth + x) myRigid.velocity = new Vector3(0, myRigid.velocity.y, myRigid.velocity.z);
        float y = cam.transform.localPosition.y;
        if (transform.localPosition.y >= Plane.MaxHeight + y) myRigid.velocity = new Vector3(myRigid.velocity.x, 0, myRigid.velocity.z);
        if (transform.localPosition.y <= -Plane.MaxHeight + y) myRigid.velocity = new Vector3(myRigid.velocity.x,0, myRigid.velocity.z);

        //clamping position
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -Plane.MaxWidth + x, Plane.MaxWidth + x), Mathf.Clamp(transform.localPosition.y, -Plane.MaxHeight + y, Plane.MaxHeight + y));
    }

    public void ApplyRotation(float addedRotation)
    {
        addedRotation *= rotationSpeed;
        largeCrossHair.Rotate(Vector3.forward, addedRotation);
        playerRotationVisuals.Rotate(Vector3.forward, addedRotation);
    }

    void updateFokusPoint(Vector3 input)
    {
        input.z = 0;
        shipFokusPoint.localPosition += input * fokusPointSpeed;


        //moving back towards (0,0):
        if (Mathf.Abs(shipFokusPoint.localPosition.x) > fokusPointCenteringTolerance && input.x == 0)
        {
            shipFokusPoint.localPosition = new Vector3(shipFokusPoint.localPosition.x * fokusPointCenteringSpeed, shipFokusPoint.localPosition.y, shipFokusPoint.localPosition.z);
        }
        if (Mathf.Abs(shipFokusPoint.localPosition.y) > fokusPointCenteringTolerance && input.y == 0)
        {
            shipFokusPoint.localPosition = new Vector3(shipFokusPoint.localPosition.x, shipFokusPoint.localPosition.y * fokusPointCenteringSpeed, shipFokusPoint.localPosition.z);
        }

        updateCrosshairPositions();
    }

    void updateCrosshairPositions()
    {
        largeCrossHair.localPosition = shipFokusPoint.localPosition + crosshairOffset;
        smallCrossHair.localPosition = shipFokusPoint.localPosition * 2 + crosshairOffset;

        /*smallCrossHair.localPosition += mouseInputs() * 0.1f;

        if (smallCrossHair.localPosition.x < -plane.maxWidth) smallCrossHair.localPosition = new Vector3(-plane.maxWidth, smallCrossHair.localPosition.y, smallCrossHair.localPosition.z);
        if (smallCrossHair.localPosition.x > plane.maxWidth) smallCrossHair.localPosition = new Vector3(plane.maxWidth, smallCrossHair.localPosition.y, smallCrossHair.localPosition.z);
        if (smallCrossHair.localPosition.y < -plane.maxHeight) smallCrossHair.localPosition = new Vector3(- smallCrossHair.localPosition.x, -plane.maxHeight, smallCrossHair.localPosition.z);
        if (smallCrossHair.localPosition.y > plane.maxHeight) smallCrossHair.localPosition = new Vector3(smallCrossHair.localPosition.x, plane.maxHeight, smallCrossHair.localPosition.z);
        */
    }

    private void clampFokusPointPosition()
    {
        shipFokusPoint.localPosition = new Vector3(Mathf.Clamp(shipFokusPoint.localPosition.x, -fokusPointRange, fokusPointRange), Mathf.Clamp(shipFokusPoint.localPosition.y, -fokusPointRange, fokusPointRange), shipFokusPoint.localPosition.z);
    }

    void gravityAndLiftEffect()
    {
        Vector3 gravity = Vector3.down * gravityAndLift;
        ApplyMovement(gravity);

        Vector3 lift = playerRotationVisuals.TransformDirection(Vector3.up) * gravityAndLift;
        ApplyMovement(lift);
    }

    public Vector3 getVelocity()
    {
        return myRigid.velocity + Plane.getVelocity();
    }

    #endregion

    /// <summary>
    /// Contains the functions related to targetmarking and launching missles
    /// </summary>
    /// <param name="input">An Enum from the inputmanager containing information about wich action to perform</param>
    #region combat

    #region Inputs
    /*
    Vector3 mouseInputs()
    {
        Vector3 retVal = Vector3.zero;
        if (Input.GetAxis("Mouse X") != 0)
        {
            retVal.x += Input.GetAxis("Mouse X");
        } 
        if (Input.GetAxis("Mouse Y") != 0)
        {
            retVal.y += Input.GetAxis("Mouse Y");
        }
        return retVal;
    }*/

    public void applyCombatInputs(INPUTS input)
    {
        switch (input)
        {
            case INPUTS.Scan:
                scanCrosshairForTarget();
                break;
            case INPUTS.SwitchTargets:
                rotateTargetList();
                break;
            case INPUTS.SwitchMissle:
                switchSelectedMissle();
                break;
        }
    }

    public void applyCombatInputs(INPUTS input, int i)
    {
        if (input == INPUTS.Missle)
        {
            FireMissle(i);
        }
    }
        #endregion

    #region health and death
    public void takeDamage(float damage)
    {
        if (invulnTimeEnd > Time.time || currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth <= 0) crash();

        UpdateHealthbar();
    }

    public void gainHealth(float repairValue)
    {
        currentHealth = Mathf.Clamp(currentHealth  + repairValue, 0, maxHealth);
        UpdateHealthbar();
    }

    void becomeInvulnerable(float t)
    {
        //avoids shortening the current cooldown
        if (invulnTimeEnd - Time.time > t) return;

        invulnTimeEnd = Time.time + t;
    }

    public void takeDamage(float damage, DamageType damageType)
    {
        //Debug.Log(damageType);

        switch (damageType)
        {
            case DamageType.highExplosive:
                takeDamage(damage);
                becomeInvulnerable(invulnTime);
                break;
            case DamageType.collision:
                takeDamage(damage);
                becomeInvulnerable(invulnTime);
                break;
            case DamageType.repairs:
                gainHealth(damage);
                Debug.Log("Healing");
                break;

            default:
                takeDamage(damage);
                break;
        }
    }

    void crash()
    {
        foreach (TurretMount tur in turretMounts)
        {
            tur.destroySelf();
        }

        currentHealth = 0;
        UpdateHealthbar();

        inGame = false;
        Invoke("destroySelf", 5);
        GameStateConnection.Instance.loosePlayer(this);

        myRigid.drag = 0;
        myRigid.AddForce(transform.forward * 12, ForceMode.Impulse);
        myRigid.useGravity = true;
        StartCoroutine(crashDetonations());
    }

    IEnumerator crashDetonations()
    {
        yield return new WaitForSeconds(0.25f);
        HelperFunctions.SpawnExplosion(crashExplosion, 1, transform.position);
        yield return new WaitForSeconds(0.15f);
        HelperFunctions.SpawnExplosion(crashExplosion, 1, transform.position);
        yield return new WaitForSeconds(0.2f);
        HelperFunctions.SpawnExplosion(crashExplosion, 1.4f, transform.position);
    }

    public void destroySelf()
    {
        HelperFunctions.SpawnExplosion(crashExplosion, 5, transform.position);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    #endregion

    #region Non-AMS-Targets
    //Layermask including Ground, player, playerbullets and other layers ignored by the target marking ray
    LayerMask layermask = 1536;
    void scanCrosshairForTarget()
    {
        float sphereCastRadius = 2f;
        Vector3 screenpos = cam.WorldToScreenPoint(smallCrossHair.position);
        Ray ray = cam.ScreenPointToRay(screenpos);
        RaycastHit hit;
        if (Physics.SphereCast(ray, sphereCastRadius, out hit, 1000, layermask))
        {
            Target t = hit.collider.GetComponent<Target>();
            if (t != null) aquireTarget(t);
        }
    }

    void rotateTargetList()
    {
        if (Targets.Count < 1) return;
        AquiredTarget targetOne = Targets[0];
        Targets.RemoveAt(0);
        Targets.Add(targetOne);

        targetsChanged();
    }

    public void aquireTarget(Target T)
    {
        //Missles are aquired automatically
        if (T.Type == TargetType.missle) return;
        CameraScript camera = cam.GetComponent<CameraScript>();

        //Checks if target is already in List
        foreach (var t in Targets)
        {
            if (t.transform == T.transform) return;
        }

        //Adds target to list 
        AquiredTarget target = new AquiredTarget(T.transform, T.getVelocity(), camera.giveLocationRelativeToCrosshair(T.transform), T.Type);
        Targets.Add(target);

        //removes oldest Target when max target count is met 
        if (Targets.Count > maxTargets) Targets.RemoveAt(0);


        targetsChanged();
    }

    void designateTargets()
    {
        CameraScript camera = cam.GetComponent<CameraScript>();

        foreach (var target in Targets)
        {
            //losing target when flying past it
            if (Plane.relativeZposition(target.transform.position) < 0)
            {
                Debug.Log("lost target");
                Targets.Remove(target);
                targetsChanged();
                return;//Targets List was modified, continuing next frame
            }

            if (target.CurrentQuarter != camera.giveLocationRelativeToCrosshair(target.transform))
            {
                target.CurrentQuarter = camera.giveLocationRelativeToCrosshair(target.transform);
                targetsChanged();
            }
        }
    }

    int amtOfTrgts = 0; //TODO: Either move to variables or replace
    void targetsChanged()
    {
        //Adding Targets to Mounts in range;

        foreach (var Mount in ATGTurrets)
        {
            Mount.clearTargets();
            foreach (var target in Targets)
            {
                if (Mount.Quarters.Contains(target.CurrentQuarter)) Mount.AddTarget(target);
            }
        }
        if(Targets.Count != amtOfTrgts)resetMarkings();
        amtOfTrgts = Targets.Count;
    }

    public void removeMarkedTarget(Target T)
    {
        foreach (var t in Targets)
        {
            if (T.transform == t.transform)
            {
                Targets.Remove(t);
                targetsChanged();
                return;
            }
        }
    }
    #endregion

    #region Hostile Missles
    void designateMissles()
    {
        if (incomingMissles.Count == 0) return;

        CameraScript camera = cam.GetComponent<CameraScript>();

        foreach (var missle in incomingMissles)
        {
            //losing target when flying past it
            if (Plane.relativeZposition(missle.transform.position) < 0)
            {
                //Debug.Log("removing missle");
                incomingMissles.Remove(missle);
                MisslesChanged();
                break; //Targets List length was modified, continuing next frame
            }

            if (missle.CurrentQuarter != camera.giveLocationRelativeToCrosshair(missle.transform))
            {
                missle.CurrentQuarter = camera.giveLocationRelativeToCrosshair(missle.transform);
                MisslesChanged();
            }
        }
    }

    void MisslesChanged()
    {
        //Clear all existing targets
        foreach (var Mount in AMSTurrets)
        {
            Mount.clearMissles();

            //Add new targets
            foreach (var missle in incomingMissles)
            {
                if (Mount.Quarters.Contains(missle.CurrentQuarter)) Mount.AddMissle(missle);
            }
            Mount.MisslesChanged();
        }
    }

    public void addIncomingMissle(Target M)
    {
        //Debug.Log("new Missle : " + M.name);
        if (M.Type != TargetType.missle) return;
        CameraScript camera = cam.GetComponent<CameraScript>();

        //Checks if target is already in List
        foreach (var t in Targets)
        {
            if (t.transform == M.transform) return;
        }

        //Adds target to list 
        AquiredTarget target = new AquiredTarget(M.transform, M.getVelocity(), camera.giveLocationRelativeToCrosshair(M.transform), M.Type);
        incomingMissles.Add(target);
        MisslesChanged();
    }

    public void removeIncomingMissle(Target M)
    {
        foreach (var m in incomingMissles)
        {
            if (M.transform == m.transform)
            {
                incomingMissles.Remove(m);
                MisslesChanged();
                return;
            }
        }
    }
    #endregion

    #region OwnMissles
    
    void FireMissle(int target)
    {
        if (Targets.Count < target + 1) return;
        //shoots a missle if there is a single turret with missles left
        for (int i = 0; i < MissleTurrets.Count; i++)
        {
            if (MissleTurrets[lastTurret].isLoaded() && MissleTurrets[lastTurret].Data.missleData == currentMissles)
            {
                MissleTurrets[lastTurret].Fire(Targets[target]);
                lastTurret = (lastTurret + 1) % MissleTurrets.Count;
                return;
            }
            lastTurret = (lastTurret + 1) % MissleTurrets.Count;
        }
    }

    void switchSelectedMissle()
    {
        int index = misslesAvailable.IndexOf(currentMissles);

        index = (index + 1) % misslesAvailable.Count;

        currentMissles = misslesAvailable[index];

        UpdateSelectedMissle();
    }
    
    
    #endregion
    #endregion


    /// <summary>
    /// used in Loadout map to change turrets
    /// </summary>
    #region Loadout
    public void AddTurretModules()
    {
        foreach (TurretMount tm in turretMounts)
        {
            TurretModule mod = tm.gameObject.AddComponent<TurretModule>();
            mod.Instantiate();
        }
    }
    
    public void RemoveTurretModules()
    {
        foreach (TurretMount tm in turretMounts)
        {
            if (tm.GetComponent<TurretModule>() != null)
            {
                tm.GetComponent<TurretModule>().startGame();
            }
        }
    }

    #endregion

    #region UpdateHUD

    #region UIMarkers
    void applyTargetMarkers()
    {
        int n = 0;
        foreach (var t in Targets)
        {
            if (t == null) return;
            TargetMarker[n].position = cam.WorldToScreenPoint(t.transform.position);
            //keeping the markers inside the screen
            if (TargetMarker[n].position.x > Screen.width) TargetMarker[n].position = new Vector3(Screen.width, TargetMarker[n].position.y);
            if (TargetMarker[n].position.x < 0) TargetMarker[n].position = new Vector3(0, TargetMarker[n].position.y);
            if (TargetMarker[n].position.y > Screen.height) TargetMarker[n].position = new Vector3(TargetMarker[n].position.x, Screen.height);
            if (TargetMarker[n].position.y < 0) TargetMarker[n].position = new Vector3(TargetMarker[n].position.x, 0);

            n++;
        }
    }

    void resetMarkings()
    {
        foreach (var marker in TargetMarker)
        {
            marker.position = new Vector3(-20, -20);
        }
    }
    #endregion


    void UpdateHealthbar()
    {
        foreach (VerticalBar item in hud.Healthbars)
        {
            item.CurrentValue = currentHealth;
        }
    }

    void UpdateSelectedMissle()
    {
        hud.MissleIcon.UpdateMissle(currentMissles);
    }


    #endregion
}
