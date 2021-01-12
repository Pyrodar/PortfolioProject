using UnityEngine;

public class AntiGroundTurret : Turret
{
    protected bool hasTarget = false;

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
        Vector3 interceptCourse = HelperFunctions.Intercept(transform.position, Vector3.zero, data.bulletData.speed, t.transform.position, t.velocity);
        LookAt(interceptCourse);
    }

    bool checkSights()
    {
        Collider objInSights = HelperFunctions.GetObjectInSights(transform.position, transform.forward, data.turretRange);

        if(objInSights == null) return false;
        if (objInSights.tag == "Enemy") return true;
        return false;
    }

    public override void Fire()
    {
        if (!isOnCooldown())
        {
            addCooldown(data.cooldown);
            //Debug.Log(this.name + " is firing!");

            GameObject b = GameObject.Instantiate(data.bulletData.visuals);
            b.transform.position = transform.position;
            b.transform.rotation = transform.rotation;

            Bullet bullet = b.AddComponent<Bullet>();
            bullet.Initialize(data.bulletData, data.bulletSpread);
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
