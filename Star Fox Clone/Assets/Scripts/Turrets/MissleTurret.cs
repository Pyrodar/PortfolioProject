using UnityEngine;

public class MissleTurret : Turret
{
    [Tooltip("how many Missles can be loaded at once")]
    private int maxMissles;
    private int currentMissles;
    public int CurrentMissles
    {
        get { return currentMissles; }
        set {
            currentMissles = value;
            if (myHudIcon != null) myHudIcon.SetMissles(currentMissles);
        }
    }

    private void Start()
    {
        maxMissles = Mathf.FloorToInt(data.missleSpace / data.missleData.missleSize);
        CurrentMissles = maxMissles;
    }

    private void Update()
    {
        loadMissles();
    }

    public override void ConnectToUI(TurretIcon icon)
    {
        base.ConnectToUI(icon);
        icon.SetMissles(currentMissles);
    }

    #region firing missle
    public void Fire(AquiredTarget target)
    {
        if (myMount.Recharging) return;

        GameObject M = GameObject.Instantiate(data.missleData.visuals);
        PlayerMissle PM = M.AddComponent<PlayerMissle>(); 
        PM.Initialize(target, data.missleData);

        M.transform.position = transform.position;
        M.transform.rotation = transform.rotation;


        float spreadF = data.ejectSpeed / 8;
        Vector3 spread = new Vector3(Random.Range(-spreadF, spreadF), Random.Range(-spreadF, spreadF), Random.Range(-spreadF, spreadF));

        //TODO: rotate through location of rockettubes
        Transform tube = transform.GetChild(0);
        //############################################

        M.GetComponent<Rigidbody>().AddForce(tube.forward * data.ejectSpeed + spread, ForceMode.Impulse); //TODO: add plane Velocity

        startReloading();
    }
    #endregion

    #region loading missles
    void loadMissles()
    {
        if (CurrentMissles < maxMissles && Time.time > cooldownEnd)
        {
            CurrentMissles += 1;
            if (CurrentMissles < maxMissles)
            {
                cooldownEnd = Time.time + data.cooldown;
                if(myHudIcon != null) myHudIcon.LoadingMissle();
            }
            //Debug.Log("Loaded new Missle");
        }
    }

    void startReloading()
    {
        CurrentMissles -= 1;
        if (Time.time > cooldownEnd)
        {
            cooldownEnd = Time.time + data.cooldown;
            myHudIcon.LoadingMissle();
        }
    }

    public bool isLoaded()
    {
        if (CurrentMissles < 1) return false;
        return true;
    }
    #endregion
}
