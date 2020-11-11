using UnityEngine;

public class AMSTurret : Turret
{
    void Update()
    {
        AquiredTarget aq = aquireTargets();
        if (aq != null)
        {
            aim(aq);
            Fire();
        }
    }

    AquiredTarget aquireTargets()
    {
        //Check for targets
        AquiredTarget M = myMount.getClosestMissle();
        if (M == null) return null;
        //Check Target Distance
        float distance = Vector3.Distance(M.transform.position, transform.position);
        if (distance > data.turretRange) return null;
        return M;
    }

    void aim(AquiredTarget M)
    {
        Vector3 targetInterception = HelperFunctions.Intercept(transform.position, Vector3.zero, data.bulletData.speed, M.transform.position, M.transform.GetComponent<EnemyMissle>().getVelocity());
        LookAt(targetInterception);
    }

    public override void Fire()
    {
        if (Time.time > cooldownEnd)
        {
            //Vector3 scatter = new Vector3(Random.Range(-data.bulletspread, data.bulletspread), Random.Range(-data.bulletspread, data.bulletspread));
            //TODO: add bulletSpread

            addCooldown(data.cooldown);

            GameObject b = GameObject.Instantiate(data.bulletData.visuals);
            b.transform.position = transform.position;
            b.transform.rotation = transform.rotation;

            Bullet bullet = b.AddComponent<Bullet>();
            bullet.Initialize(data.bulletData);
        }
    }

    void addCooldown(float t)
    {
        //avoids shortening the current cooldown
        if (cooldownEnd - Time.time > t) return;

        cooldownEnd = Time.time + t;
    }
}
