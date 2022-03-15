using UnityEngine;

public class ATGTurret : Turret
{
    protected bool hasTarget = false;

    float flakDelay;
    Vector3 interceptPoint = Vector3.zero;

    void Update()
    {
        aim(aquireTarget());

        if (data.firingDiscipline)
        {
            //Checks if line of sight lines up before firing
            if(hasTarget && checkSights()) Fire();
        }
        else
        {
            //fires regardles of what the turret aims at
            if (hasTarget) Fire();
        }
    }

    AquiredTarget aquireTarget()
    {
        return myMount.getPriorityTarget();
    }

    void aim(AquiredTarget t)
    {
        #region rotationBuffer
        //prevents turrets from shooting before they had time to rotate towards new Target
        //could be replaced by new improved checkSights function, but might take away from overall atmosphere
        if (t == null)
        {
            if (hasTarget) hasTarget = false;
            return;
        }
        else if (!hasTarget && data.firingDiscipline)
        {
            hasTarget = true;
        }
        else if (!hasTarget)
        {
            hasTarget = true;
            addCooldown(data.cooldown / 3);
        }
        #endregion

        t.UpdateVelocity();
        interceptPoint = HelperFunctions.Intercept(transform.position, myPlayer.Velocity, data.bulletData.speed, t.transform.position, t.Velocity);
        LookAt(interceptPoint);

        //Check Target Distance
        float distance = Vector3.Distance(transform.position, interceptPoint);
        if (data.bulletData.damageType == DamageType.flak)
        {
            flakDelay = distance / data.bulletData.speed;
        }
    }

    bool checkSights()
    {
        return HelperFunctions.LinedUp(interceptPoint, transform.position, transform.forward);
    }

    public override void Fire()
    {
        if (!isOnCooldown())
        {
            addCooldown(data.cooldown);
            //Debug.Log(this.name + " is firing!");

            //Networking/////////////////////////
            var smallData = data.GetSmallData();
            myPlayer.CmdSpawnBullet(smallData, transform.position, transform.rotation, flakDelay);
            /////////////////////////////////////
        }
    }

    bool isOnCooldown()
    {
        if (Time.time > cooldownEnd) return false;
        return true;
    }

    void addCooldown(float t)
    {
        //avoids shortening the current cooldown
        if (cooldownEnd - Time.time > t) return;

        cooldownEnd = Time.time + t;
    }
}
