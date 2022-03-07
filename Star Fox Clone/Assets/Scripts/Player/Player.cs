using ProtocFiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

//Maybe rename to chassis and remove Input stuff
public class Player : MonoBehaviour , IVehicle
{
    /// <summary>
    /// Everything needed for the game to work
    /// </summary>
    #region SetOnLevelLoaded

    bool isPuppet;
    public bool IsPuppet { get { return isPuppet; } }
    public bool MakePuppet { set { isPuppet = value; } }

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
    RectTransform canvas { get { return hud.GetComponent<RectTransform>(); } }

    //required for splitscreen
    int playerNumber;

    #endregion

    /// <summary>
    /// All variables that affect the Gameplay feeling
    /// TODO: NEEDS TO BE INTERCHANGABLE BASED ON SHIPTYPE!!!
    /// </summary>
    #region Shiptype dependent

    /// <summary>
    ///Required for the game to work and saved before starting the game in the prefab
    ///TODO: Visuals and Hitbox must be interchangable based on ship type
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
    #endregion

    [SerializeField] ShipData shipData;
    public ShipData ShipData { get { return shipData; } }

    #region private variables
    Rigidbody myRigid;

    
    #endregion

    #region Combat
    float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
    }
    float invulnTimeEnd = 1;
    #endregion
    
    #endregion

    /// <summary>
    /// will be set automatically when the Game starts
    /// </summary>
    #region Targets and Turrets
    List<TurretMount> AMSTurrets = new List<TurretMount>();
    List<TurretMount> ATGTurrets = new List<TurretMount>();
    List<MSLTurret> MSLTurrets = new List<MSLTurret>();
    int lastTurret = 0; //Used to cycle MissleTurrets

    List<MissleData> misslesAvailable = new List<MissleData>();
    MissleData currentMissles;

    List<AquiredTarget> Targets = new List<AquiredTarget>();
    List<AquiredTarget> incomingMissles = new List<AquiredTarget>();
    private int maxTargets = 3;

    public Vector3 Velocity
    {
        get { return myRigid.velocity + Plane.getVelocity(); }
    }
    #endregion


    #region HUD
    VerticalBar[] healthbars;
    [SerializeField] RectTransform[] TargetMarker;
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
        currentHealth = shipData.maxHealth;
        healthbars = hud.Healthbars;

        foreach (VerticalBar item in healthbars)
        {
            item.Initialize(shipData.maxHealth);
        }


        TurretIconList list = hud.TurretIconList;


        for (int i = 0; i < 3; i++)
        {
            TargetMarker[i] = hud.TargetMarkers[i];
        }

        resetMarkings();
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
            tur.PlayerReferenz = this;      //Referenz to get Player Velocity
            if (tur.MyTurretType == TurretClass_P.Atg) ATGTurrets.Add(tur);
            if (tur.MyTurretType == TurretClass_P.Ams) AMSTurrets.Add(tur);
            if (tur.MyTurretType == TurretClass_P.Msl)
            {
                MSLTurrets.Add(tur.getMissleTurret());

                //Listing all available MissleTypes
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

    public void SetPlayerNumber(int i)
    {
        playerNumber = i;
        largeCrossHair.gameObject.layer = 13 + i;
        smallCrossHair.gameObject.layer = 13 + i;
    }

    /// <summary>
    /// Adjusting plane transform based on Fokuspoint position
    /// recalculates target locations and updates HUD accordingly
    /// applies gravity and lift
    /// </summary>
    void Update()
    {
        if (!inGame) return;

        HelperFunctions.LookAt(playerVisuals, shipFokusPoint.position, shipData.fokusPointDamping);

        gravityAndLiftEffect();

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
        if (isPuppet) return;

        //Input is set relative to framerate
        input *= Time.deltaTime;
        myRigid.AddRelativeForce(input * shipData.moveSpeed);
        updateFokusPoint(input);

        checkBoundaries();
        clampFokusPointPosition();
    }

    void checkBoundaries()
    {

        //setting velocity to 0 slowly when hitting boundries so enemy Missles and AA don't get confused
        float x = cam.transform.localPosition.x;
        if (transform.localPosition.x >= Plane.MaxWidth + x) myRigid.velocity = new Vector3(myRigid.velocity.x - (myRigid.velocity.x * Time.deltaTime), myRigid.velocity.y, myRigid.velocity.z);
        if (transform.localPosition.x <= -Plane.MaxWidth + x) myRigid.velocity = new Vector3(myRigid.velocity.x - (myRigid.velocity.x * Time.deltaTime), myRigid.velocity.y, myRigid.velocity.z);
        float y = cam.transform.localPosition.y;
        if (transform.localPosition.y >= Plane.MaxHeight + y) myRigid.velocity = new Vector3(myRigid.velocity.x, myRigid.velocity.y - (myRigid.velocity.y * Time.deltaTime), myRigid.velocity.z);
        if (transform.localPosition.y <= -Plane.MaxHeight + y) myRigid.velocity = new Vector3(myRigid.velocity.x, myRigid.velocity.y - (myRigid.velocity.y * Time.deltaTime), myRigid.velocity.z);

        //clamping position
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -Plane.MaxWidth * 2, Plane.MaxWidth * 2), Mathf.Clamp(transform.localPosition.y, -Plane.MaxHeight * 2, Plane.MaxHeight * 2));


        //if (transform.localPosition.z != 0)
        //{
        //    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, plane.Playerpositions[0].position.z);
        //}
    }

    public void ApplyRotation(float addedRotation)
    {
        if (isPuppet) return;

        //Input is made relative to framerate
        addedRotation *= Time.deltaTime;
        addedRotation *= shipData.rotationSpeed;
        largeCrossHair.Rotate(Vector3.forward, addedRotation);
        playerRotationVisuals.Rotate(Vector3.forward, addedRotation);
    }

    void updateFokusPoint(Vector3 input)
    {
        input.z = 0;
        shipFokusPoint.localPosition += input * shipData.fokusPointSpeed;


        //moving focus point back towards local (0,0):

        if (Mathf.Abs(shipFokusPoint.localPosition.x) > shipData.fokusPointCenteringTolerance && Mathf.Abs(input.x) < .1f)
        {
            shipFokusPoint.localPosition = new Vector3( shipFokusPoint.localPosition.x - ((shipFokusPoint.localPosition.x * shipData.fokusPointCenteringSpeed) * Time.deltaTime), 
                                                        shipFokusPoint.localPosition.y, 
                                                        shipFokusPoint.localPosition.z);
        }


        if (Mathf.Abs(shipFokusPoint.localPosition.y) > shipData.fokusPointCenteringTolerance && Mathf.Abs(input.y) < .1f)
        {
            shipFokusPoint.localPosition = new Vector3( shipFokusPoint.localPosition.x, 
                                                        shipFokusPoint.localPosition.y - ((shipFokusPoint.localPosition.y * shipData.fokusPointCenteringSpeed) * Time.deltaTime), 
                                                        shipFokusPoint.localPosition.z);
        }

        updateCrosshairPositions();
    }

    void updateCrosshairPositions()
    {
        largeCrossHair.localPosition = shipFokusPoint.localPosition + shipData.crosshairOffset;
        smallCrossHair.localPosition = shipFokusPoint.localPosition * 2 + shipData.crosshairOffset;
    }

    private void clampFokusPointPosition()
    {
        shipFokusPoint.localPosition = new Vector3(Mathf.Clamp(shipFokusPoint.localPosition.x, -shipData.fokusPointRange, shipData.fokusPointRange), Mathf.Clamp(shipFokusPoint.localPosition.y, -shipData.fokusPointRange, shipData.fokusPointRange), shipFokusPoint.localPosition.z);
    }

    void gravityAndLiftEffect()
    {
        Vector3 gravity = Vector3.down * shipData.gravityAndLift * Time.deltaTime;
        ApplyMovement(gravity);

        Vector3 lift = playerRotationVisuals.TransformDirection(Vector3.up) * shipData.gravityAndLift * Time.deltaTime;
        ApplyMovement(lift);
    }

    #endregion

    /// <summary>
    /// Contains the functions related to targetmarking and launching missles
    /// </summary>
    /// <param name="input">An Enum from the inputmanager containing information about wich action to perform</param>
    #region combat

    #region Inputs

    public void applyCombatInputs(INPUTS input)
    {
        if (isPuppet) return;

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
        currentHealth = Mathf.Clamp(currentHealth  + repairValue, 0, shipData.maxHealth);
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
                becomeInvulnerable(shipData.invulnTime);
                break;
            case DamageType.collision:
                takeDamage(damage);
                becomeInvulnerable(shipData.invulnTime);
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
        GameConnection.Instance.loosePlayer(this);

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
    //Layermask including only Environment and Enemies
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
        AquiredTarget target = new AquiredTarget(T.transform, T.Velocity, camera.giveLocationRelativeToCrosshair(T.transform), T.Type);
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
        AquiredTarget target = new AquiredTarget(M.transform, M.Velocity, camera.giveLocationRelativeToCrosshair(M.transform), M.Type);
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
    
    public void FireMissle(int target)
    {
        if (isPuppet) return;

        if (Targets.Count < target + 1) return;
        //shoots a missle if there is a single turret with missles left
        for (int i = 0; i < MSLTurrets.Count; i++)
        {
            if (MSLTurrets[lastTurret].isLoaded() && MSLTurrets[lastTurret].Data.missleData == currentMissles)
            {
                MSLTurrets[lastTurret].Fire(Targets[target]);
                lastTurret = (lastTurret + 1) % MSLTurrets.Count;
                return;
            }
            lastTurret = (lastTurret + 1) % MSLTurrets.Count;
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
    /// creates a list of all Modules and numerates them
    /// used in Loadout map to change turrets
    /// ModuleNumbers are used for the savefiles
    /// </summary>
    #region Loadout
    public void AddTurretModules(LoadoutHUD HUD)
    {
        List<TurretModule> mods = new List<TurretModule>();

        int moduleNumber = 0;
        foreach (TurretMount tm in turretMounts)
        {
            TurretModule mod = tm.gameObject.AddComponent<TurretModule>();
            mod.Instantiate(playerNumber);

            mod.ModuleNumber = moduleNumber++;

            mods.Add(mod);
        }

        HUD.SetModuleList = mods;
    }
    
    public void RemoveTurretModules()
    {
        foreach (TurretMount tm in turretMounts)
        {
            tm.gameObject.layer = 2;       //Ignore Raycast Layer
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


            Vector2 screenPos = cam.WorldToScreenPoint(t.transform.position);

            //used for vertical splitscreen. using "if" instead of "switch" since only 2 players are possible
            if (playerNumber != 0) screenPos.x -= canvas.rect.width;// * playerNumber;

            //TODO: set up for horizontal splitscreen 
            //if (playerNumber != 0) screenPos.y -= canvas.rect.height;// * playerNumber;


            TargetMarker[n].anchoredPosition = screenPos;
            //keeping the markers inside the screen

            if (TargetMarker[n].anchoredPosition.x > canvas.rect.width) TargetMarker[n].anchoredPosition = new Vector3(canvas.rect.width, TargetMarker[n].anchoredPosition.y);
            if (TargetMarker[n].anchoredPosition.x < 0) TargetMarker[n].anchoredPosition = new Vector3(0, TargetMarker[n].anchoredPosition.y);
            if (TargetMarker[n].anchoredPosition.y > canvas.rect.height) TargetMarker[n].anchoredPosition = new Vector3(TargetMarker[n].anchoredPosition.x, canvas.rect.height);
            if (TargetMarker[n].anchoredPosition.y < 0) TargetMarker[n].anchoredPosition = new Vector3(TargetMarker[n].anchoredPosition.x, 0);

            n++;
        }
    }

    void resetMarkings()
    {
        foreach (var marker in TargetMarker)
        {
            marker.anchoredPosition = new Vector3(-2000, -2000);
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

    #region NetworkSpawning
    public void CmdSpawnBullet(SmallTurretData data, Vector3 position, Quaternion rotation, float flakDelay)
    {

        BulletFactory factory = MapLayoutInfo.Instance.BulletFactory;

        factory.CmdSpawnBullet(BulletOrigin.AMS, data.bulletData, position, rotation, data.bulletSpread, flakDelay);

        //GameObject b = GameObject.Instantiate(data.bulletData.visuals);
        //b.transform.position = position;
        //b.transform.rotation = rotation;

        //Bullet bullet = b.AddComponent<Bullet>();
        //bullet.tag = "AMSBullet";

        //if (data.bulletData.damageType == DamageType.flak)                                                          //TODO: add player Velocity to bullet
        //{
        //    bullet.Initialize(data.bulletData, data.bulletSpread, BulletOrigin.Player, Vector3.zero, flakDelay);    //setting bullet timer manually for flak ammunition
        //}
        //else bullet.Initialize(data.bulletData, data.bulletSpread, BulletOrigin.Player, Vector3.zero);              //using regular bullet timer

    }

    void RpcSpawnBullet(SmallTurretData data, Vector3 position, Quaternion rotation, float flakDelay)
    {

    }

    public void CmdSpawnMissle(SmallTurretData data, Vector3 position, Quaternion rotation, AquiredTarget target)
    {
        GameObject M = GameObject.Instantiate(data.missleData.visuals);
        PlayerMissle PM = M.AddComponent<PlayerMissle>();
        PM.Initialize(target, data.missleData);

        M.transform.position = position;
        M.transform.rotation = rotation;


        float spreadF = data.ejectSpeed / 8;
        Vector3 spread = new Vector3(Random.Range(-spreadF, spreadF), Random.Range(-spreadF, spreadF), Random.Range(-spreadF, spreadF));

        M.GetComponent<Rigidbody>().AddForce(M.transform.forward * data.ejectSpeed + spread + Velocity, ForceMode.Impulse);
    }

    void RpcSpawnMissle(SmallTurretData data, Vector3 position, Quaternion rotation, AquiredTarget target)
    {

    }

    #endregion
}
