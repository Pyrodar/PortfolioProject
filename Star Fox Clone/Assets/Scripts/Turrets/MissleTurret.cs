using UnityEngine;

public class MissleTurret : Turret
{
    [Tooltip("how many Missles can be loaded at once")]
    private int maxMissles;
    private int currentMissles;

    private void Start()
    {
        maxMissles = Mathf.FloorToInt(data.missleSpace / data.missleData.missleSize);
        currentMissles = maxMissles;
    }

    private void Update()
    {
        loadMissles();
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

        M.GetComponent<Rigidbody>().AddForce(tube.forward * data.ejectSpeed + spread, ForceMode.Impulse);

        startReloading();
    }
    #endregion

    #region loading missles
    void loadMissles()
    {
        if (currentMissles < maxMissles && Time.time > cooldownEnd)
        {
            currentMissles += 1;
            if (currentMissles < maxMissles) cooldownEnd = Time.time + data.cooldown;
            //Debug.Log("Loaded new Missle");
        }
    }

    void startReloading()
    {
        currentMissles -= 1;
        if (Time.time > cooldownEnd)
        {
            cooldownEnd = Time.time + data.cooldown;
        }
    }

    public bool isLoaded()
    {
        if (currentMissles < 1) return false;
        return true;
    }
    #endregion
}
