using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour , IVehicle
{
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
    bool gameStarted;
    public bool GameStarted
    {
        set { gameStarted = value; }
    }
    #endregion

    #region SetManuallyInEditor
    [SerializeField] TurretMount[] turretMounts;
    public TurretMount[] TurretMounts
    {
        get { return turretMounts; }
    }

    [SerializeField] Transform playerVisuals;
    [SerializeField] Transform playerRotationVisuals;
    [SerializeField] Transform lookAtThis;
    [SerializeField] Transform smallCrossHair;
    [SerializeField] Transform largeCrossHair;
    public Transform LargeCrosshair { get { return largeCrossHair; } }
    [SerializeField] GameObject crashExplosion;
    Rigidbody myRigid;
    #endregion

    #region Movement
    [SerializeField] float moveSpeed = 2.6f;
    [SerializeField] float rotationSpeed = 0.7f;

    [SerializeField] float lookAtThisRange = 2f;
    [SerializeField] float lookAtThisSpeed = 0.04f;
    [SerializeField] float lookAtThisDamping = 2f; 
    [SerializeField]
    [Tooltip("how fast the ship returns to looking straight forward again. fastest: 0.0 slowest: 1.0")]
    [Range(0f, 1f)]
    float lookAtThisCenteringSpeed = 0.98f;
    [SerializeField]
    [Tooltip("how close to the ships front the crosshair returns after stopping the inputs")]
    float lookAtThisCenteringTolerance = 0.01f;

    [SerializeField] float gravityAlsoLift = 0.4f;

    [SerializeField] Vector3 crosshairoffset;
    #endregion

    #region Combat
    [SerializeField] float maxHealth = 100;
    float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    #region Targets and Turrets

    List<TurretMount> ATGTurrets = new List<TurretMount>();
    List<TurretMount> AMSTurrets = new List<TurretMount>();
    List<MissleTurret> MissleTurrets = new List<MissleTurret>();
    int lastTurret = 0;

    List<AquiredTarget> Targets = new List<AquiredTarget>();
    List<AquiredTarget> incomingMissles = new List<AquiredTarget>();
    private int maxTargets = 3;
    [SerializeField] Transform[] TargetMarker;
    #endregion

    #endregion

    #region UI
    VerticalBar healthbar;
    #endregion

    #region Debugging
    bool DPadBool = true;

    private void OnMouseDown()
    {
        Debug.Log("Klicked on: " + name);
    }
    #endregion

    public void StartGame(GameplayPlane _plane)
    {
        plane = _plane;

        #region Health
        Debug.Log("Setting Health");

        currentHealth = maxHealth;
        healthbar = UserInterface.Instance.Healthbar;
        healthbar.Initialize(maxHealth);
        #endregion

        #region Rigidbody

        Debug.Log("Adding RigidBody");
        //Rigidbody messes with the TurretColliders, therefore it's only added once the Game Starts
        myRigid = this.gameObject.AddComponent<Rigidbody>();
        myRigid.mass = 1;
        myRigid.drag = 2;
        myRigid.useGravity = false;
        myRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY| RigidbodyConstraints.FreezeRotationZ;

        #endregion

        #region Turrets
        Debug.Log("Listing Turrets");

        TurretIconList list = UserInterface.Instance.TurretIconList;

        foreach (var tur in turretMounts)
        {
            list.AddTurretToList(tur);
            if (tur.MyTurretType == TurretType.AntiGround) ATGTurrets.Add(tur);
            if (tur.MyTurretType == TurretType.AMS) AMSTurrets.Add(tur);
            if (tur.MyTurretType == TurretType.Missiles) MissleTurrets.Add(tur.getMissleTurret());
        }
        #endregion

        gameStarted = true;
    }

    void Update()
    {
        if (!gameStarted) return;
        //Movement
        applyMovement(inputMovement());

        checkBoundaries();

        applyRotation(inputRotation());

        HelperFunctions.LookAt(playerVisuals, lookAtThis.position, lookAtThisDamping);
        //VisualsLookAt(lookAtThis);

        clampLookAtThis();

        gravityAndLift();

        //Combat
        combatInputs();

        applyTargetMarkers();

        designateTargets();

        designateMissles();
    }

    #region Movement
    Vector3 inputMovement()
    {
        Vector3 retVal = Vector3.zero;

        if (Input.GetAxis("Horizontal") != 0)
        {
            retVal += new Vector3(Input.GetAxis("Horizontal") * moveSpeed, 0, 0);
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            retVal += new Vector3(0, Input.GetAxis("Vertical") * moveSpeed, 0);
        }

        return retVal;
    }

    void applyMovement(Vector3 input)
    {
        myRigid.AddForce(input * moveSpeed);
        updateLookDirection(input);
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

    float inputRotation()
    {
        float retVal = 0;

        if (Input.GetAxis("Rotation") != 0)
        {
            //Debug.Log("Added rotation: " + Input.GetAxis("Rotation"));
            retVal += Input.GetAxis("Rotation") * rotationSpeed;
        }
        return retVal;
    }

    void applyRotation(float addedRotation)
    {
        largeCrossHair.Rotate(Vector3.forward, addedRotation);
        playerRotationVisuals.Rotate(Vector3.forward, addedRotation);
    }

    void updateLookDirection(Vector3 input)
    {
        input.z = 0;
        lookAtThis.localPosition += input * lookAtThisSpeed;


        //moving back towards (0,0):
        if (Mathf.Abs(lookAtThis.localPosition.x) > lookAtThisCenteringTolerance && input.x == 0)
        {
            lookAtThis.localPosition = new Vector3(lookAtThis.localPosition.x * lookAtThisCenteringSpeed, lookAtThis.localPosition.y, lookAtThis.localPosition.z);
        }
        if (Mathf.Abs(lookAtThis.localPosition.y) > lookAtThisCenteringTolerance && input.y == 0)
        {
            lookAtThis.localPosition = new Vector3(lookAtThis.localPosition.x, lookAtThis.localPosition.y * lookAtThisCenteringSpeed, lookAtThis.localPosition.z);
        }

        updateCrosshairPositions();
    }

    void updateCrosshairPositions()
    {
        largeCrossHair.localPosition = lookAtThis.localPosition + crosshairoffset;
        smallCrossHair.localPosition = lookAtThis.localPosition * 2 + crosshairoffset;
        /*smallCrossHair.localPosition += mouseInputs() * 0.1f;

        if (smallCrossHair.localPosition.x < -plane.maxWidth) smallCrossHair.localPosition = new Vector3(-plane.maxWidth, smallCrossHair.localPosition.y, smallCrossHair.localPosition.z);
        if (smallCrossHair.localPosition.x > plane.maxWidth) smallCrossHair.localPosition = new Vector3(plane.maxWidth, smallCrossHair.localPosition.y, smallCrossHair.localPosition.z);
        if (smallCrossHair.localPosition.y < -plane.maxHeight) smallCrossHair.localPosition = new Vector3(- smallCrossHair.localPosition.x, -plane.maxHeight, smallCrossHair.localPosition.z);
        if (smallCrossHair.localPosition.y > plane.maxHeight) smallCrossHair.localPosition = new Vector3(smallCrossHair.localPosition.x, plane.maxHeight, smallCrossHair.localPosition.z);
        */
    }

    private void clampLookAtThis()
    {
        lookAtThis.localPosition = new Vector3(Mathf.Clamp(lookAtThis.localPosition.x, -lookAtThisRange, lookAtThisRange), Mathf.Clamp(lookAtThis.localPosition.y, -lookAtThisRange, lookAtThisRange), lookAtThis.localPosition.z);
    }

    void gravityAndLift()
    {
        Vector3 gravity = Vector3.down * gravityAlsoLift;
        applyMovement(gravity);

        Vector3 lift = playerRotationVisuals.TransformDirection(Vector3.up) * gravityAlsoLift;
        applyMovement(lift);
    }

    public Vector3 getVelocity()
    {
        return myRigid.velocity + Plane.getVelocity();
    }

    #endregion

    #region combat

    void combatInputs()
    {
        if (Input.GetButton("Mark")) scanCrosshairForTarget();

        if (Input.GetButtonDown("SwitchTargets")) rotateTargetList();
        //NumButtons
        if (Input.GetButtonDown("Missle1")) FireMissle(0);
        if (Input.GetButtonDown("Missle2")) FireMissle(1);
        if (Input.GetButtonDown("Missle3")) FireMissle(2);
        //Dpad axis TODO: called every frame after pushing axis down
        if (Input.GetAxisRaw("DPad X") < -0.1) {if(DPadBool) FireMissle(0); DPadBool = false;}
        else if (Input.GetAxisRaw("DPad X") > 0.1) {if (DPadBool) FireMissle(1); DPadBool = false;}
        else if (Input.GetAxisRaw("DPad Y") > 0.1) {if (DPadBool) FireMissle(2); DPadBool = false;}
        else DPadBool = true;
    }

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
    }

    public void takeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        //Debug.Log("CurrentHealth = " + currentHealth);
        if (currentHealth <= 0) crash();

        UpdateHealthbar();
    }

    void UpdateHealthbar()
    {
        healthbar.CurrentValue = currentHealth;
    }

    void crash()
    {
        Invoke("destroySelf", 5);
        gameStarted = false;
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
        Destroy(gameObject);
    }

    #region Targets
    LayerMask layermask = 1536;
    void scanCrosshairForTarget()
    {
        float sphereCastRadius = 2f;
        Vector3 screenpos = cam.WorldToScreenPoint(smallCrossHair.position);
        Ray ray = cam.ScreenPointToRay(screenpos);
        RaycastHit hit;
        if (Physics.SphereCast(ray, sphereCastRadius, out hit, 1000, layermask))
        {
            Target t = hit.collider.gameObject.GetComponent<Target>();
            if (t == null) return;
            aquireTarget(t);
        }
    }

    void rotateTargetList()
    {
        if (Targets.Count < 1) return;
        AquiredTarget targetOne = Targets[0];
        Targets.RemoveAt(0);
        Targets.Add(targetOne);

        TargetsChanged();
    }

    public void aquireTarget(Target T)
    {
        //Missles are aquired automatically
        if (T.type == TargetType.missle) return;
        CameraScript camera = cam.GetComponent<CameraScript>();

        //Checks if target is already in List
        foreach (var t in Targets)
        {
            if (t.transform == T.transform) return;
        }

        //Adds target to list 
        AquiredTarget target = new AquiredTarget(T.transform, T.getVelocity(), camera.giveLocationRelativeToCrosshair(T.transform), T.type);
        Targets.Add(target);

        //removes oldest Target when max target count is met 
        if (Targets.Count > maxTargets) Targets.RemoveAt(0);


        TargetsChanged();
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
                TargetsChanged();
                return;//Targets List was modified, continuing next frame
            }

            if (target.currentQuarter != camera.giveLocationRelativeToCrosshair(target.transform))
            {
                target.currentQuarter = camera.giveLocationRelativeToCrosshair(target.transform);
                TargetsChanged();
            }
        }
    }

    static int amtOfTrgts;
    void TargetsChanged()
    {
        //Adding Targets to Mounts in range;

        foreach (var Mount in ATGTurrets)
        {
            Mount.clearTargets();
            foreach (var target in Targets)
            {
                if (Mount.Quarters.Contains(target.currentQuarter)) Mount.AddTarget(target);
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
                TargetsChanged();
                return;
            }
        }
    }
    #endregion

    #region Missles
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
                return;//Targets List was modified, continuing next frame
            }

            if (missle.currentQuarter != camera.giveLocationRelativeToCrosshair(missle.transform))
            {
                missle.currentQuarter = camera.giveLocationRelativeToCrosshair(missle.transform);
                MisslesChanged();
            }
        }
    }

    void MisslesChanged()
    {
        //Debug.Log("Missles changed / Missle Location changed");
        foreach (var Mount in AMSTurrets)
        {
            Mount.clearMissles();
            foreach (var missle in incomingMissles)
            {
                if (Mount.Quarters.Contains(missle.currentQuarter)) Mount.AddMissle(missle);
            }
        }
    }

    public void addIncomingMissle(Target M)
    {
        //Debug.Log("new Missle : " + M.name);
        if (M.type != TargetType.missle) return;
        CameraScript camera = cam.GetComponent<CameraScript>();

        //Checks if target is already in List
        foreach (var t in Targets)
        {
            if (t.transform == M.transform) return;
        }

        //Adds target to list 
        AquiredTarget target = new AquiredTarget(M.transform, M.getVelocity(), camera.giveLocationRelativeToCrosshair(M.transform), M.type);
        incomingMissles.Add(target);
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
        foreach (var mtur in MissleTurrets)
        {
            if (MissleTurrets[lastTurret].isLoaded())
            {
                MissleTurrets[lastTurret].Fire(Targets[target]);
                lastTurret = (lastTurret + 1) % MissleTurrets.Count;
                return;
            }
            lastTurret = (lastTurret + 1) % MissleTurrets.Count;
        }
    }
    
    
    #endregion

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

    #endregion

}
