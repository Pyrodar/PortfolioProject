using UnityEngine;

public class AMSTurret : Turret
{
    float maximumHeat = 100;
    float currentHeat = 0;

    bool overheated = false;
    AquiredTarget aquiredTarget;
    AquiredTarget prevTarget = null;

    float flakDelay;

    void Update()
    {
        if (aquiredTarget != null && aim(aquiredTarget))
        {
            Fire();
        }
        else MisslesChanged();
        coolWeapon();
    }


    #region Targets

    AquiredTarget aquireTargets()
    {
        //Check for targets
        AquiredTarget M;

        if(data.bulletData.damageType == DamageType.flak) M = myMount.getClosestMissleFlak(75); //minimum range
        else M = myMount.getClosestMissle();

        if (M == null) return null;

        //pause shortly to switch targets
        if (M != prevTarget)
        {
            prevTarget = M;
            addCooldown(data.cooldown);
        }

        //aquire target
        return M;
    }

    public void MisslesChanged()
    {
        //retargeting
        aquiredTarget = aquireTargets();
    }

    bool aim(AquiredTarget M)
    {
        //Play Sound

        M.UpdateVelocity();
        Vector3 interceptPoint = HelperFunctions.Intercept(transform.position, myPlayer.Velocity, data.bulletData.speed, M.transform.position, M.Velocity);
        LookAt(interceptPoint);//TODO: add rotation of target to Intercept funktion

        //Check Target Distance
        float distance = Vector3.Distance(transform.position, interceptPoint);
        if (data.bulletData.damageType == DamageType.flak)
        {
            flakDelay = distance / data.bulletData.speed;
        }

        if (distance > data.turretRange) return false;

        return true;
    }

    #endregion

    #region shoot
    public override void Fire()
    {
        if (Time.time > cooldownEnd && !overheated)
        {

            addCooldown(data.cooldown);

            //Networking/////////////////////////
            var smallData = data.GetSmallData();
            myPlayer.CmdSpawnBullet(smallData, transform.position, transform.rotation, flakDelay);
            /////////////////////////////////////

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
        currentHeat += data.heatBuildup;

        if (currentHeat > maximumHeat * .6f) myHudIcon.ShowHeatWarning(true);   //Start Warning
        if (currentHeat > maximumHeat) overheat(true);                          //Start Overheat
    }

    void coolWeapon()
    {
        #region reduce Heat
        if (currentHeat <= 0) return;

        if(overheated) currentHeat -= Time.deltaTime * data.heatDissipation * 3f;
        else currentHeat -= Time.deltaTime * data.heatDissipation;
        #endregion

        if (!overheated && currentHeat < maximumHeat * .6f) myHudIcon.ShowHeatWarning(false);   //Stop Warning
        if (overheated && currentHeat < maximumHeat / 4) overheat(false);                       //Stop Overheat
    }

    void overheat(bool value)
    {
        overheated = value;
        myHudIcon.ShowHeatShutdown(value);
    }

    #endregion

}
