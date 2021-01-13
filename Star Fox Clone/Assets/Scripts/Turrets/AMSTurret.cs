using UnityEngine;

public class AMSTurret : Turret
{
    float maximumHeat = 100;
    float currentHeat = 0;

    float heatBuildup = 1f;
    float heatDissipation = 5f;

    bool overheated = false;


    void Update()
    {
        AquiredTarget at = aquireTargets();
        if (at != null)
        {
            aim(at);
            Fire();
        }
        coolWeapon();
    }


    #region Targets

    AquiredTarget aquireTargets()
    {
        //Check for targets
        AquiredTarget M = myMount.getClosestMissle();
        if (M == null) return null;

        //Check Target Distance
        float distance = Vector3.Distance(M.transform.position, transform.position);
        if (distance > data.turretRange) return null;

        //aquire target
        M.gunsPointing += 1;
        return M;
    }

    #endregion

    #region shoot

    void aim(AquiredTarget M)
    {
        M.UpdateVelocity();
        Vector3 targetInterception = HelperFunctions.Intercept(transform.position, Vector3.zero, data.bulletData.speed, M.transform.position, M.transform.GetComponent<EnemyMissle>().getVelocity());
        LookAt(targetInterception);
    }

    public override void Fire()
    {
        if (Time.time > cooldownEnd && !overheated)
        {
            //Vector3 scatter = new Vector3(Random.Range(-data.bulletspread, data.bulletspread), Random.Range(-data.bulletspread, data.bulletspread));
            //TODO: add bulletSpread

            addCooldown(data.cooldown);

            GameObject b = GameObject.Instantiate(data.bulletData.visuals);
            b.transform.position = transform.position;
            b.transform.rotation = transform.rotation;

            Bullet bullet = b.AddComponent<Bullet>();
            bullet.Initialize(data.bulletData, data.bulletSpread);

            applyHeat();
        }
    }

    void addCooldown(float t)
    {
        //avoids shortening the current cooldown
        if (cooldownEnd - Time.time > t) return;

        cooldownEnd = Time.time + t;
    }

    #endregion

    #region Heat

    void applyHeat()
    {
        currentHeat += heatBuildup;
        if (currentHeat > maximumHeat * .6f) myHudIcon.ShowHeatWarning();
        if (currentHeat > maximumHeat) overheat(true);
    }

    void coolWeapon()
    {
        if (currentHeat <= 0) return;

        if(overheated) currentHeat -= Time.deltaTime * heatDissipation * 3f;
        else currentHeat -= Time.deltaTime * heatDissipation;

        if (overheated && currentHeat < maximumHeat / 4)
        {
            overheat(false);
        }

        //Debug.Log($"{name} current Heat is at: {currentHeat}");
    }

    void overheat(bool value)
    {
        overheated = value;
        myHudIcon.ShowHeatShutdown(value);
        
        //Debug.Log($"Weapon is overheated: {value}");
    }

    #endregion

}
