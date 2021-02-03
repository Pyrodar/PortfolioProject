using System.Collections;
using UnityEngine;

public class MissleLauncher : StationaryWeapon
{
    /// <summary>
    /// Aims above the player, so the missles can descend into the AMS cover
    /// </summary>
    protected override void aim()
    {
        Vector3 offset = new Vector3(0, 5, 0);

        Vector3 InterceptPoint = getInterceptPoint();
        if (InterceptPoint.magnitude == 0) return;

        HelperFunctions.LookAt(transform, InterceptPoint + offset, data.turretSpeed, RotationParent.up);
    }

    /// <summary>
    /// uses Missle speed
    /// </summary>
    /// <returns></returns>
    protected override Vector3 getInterceptPoint()
    {
        return HelperFunctions.Intercept(transform.position, Vector3.zero, data.missleData.speed, myTarget.transform.position, myTarget.Velocity);
    }

    protected override IEnumerator Fire()
    {
        Vector3 ic = getInterceptPoint();

        //checking for free LOS
        float dist = Vector3.Distance(transform.position, ic) * 0.75f;  //Reducing range to avoid being blocked by objects around the plane
        Collider c = HelperFunctions.GetObjectInSights(transform.position, ic, dist);
        if (c != null) yield return null;

        //waiting for the turret to turn. randomizing it to desynchronize enemies
        if(synchronized) yield return new WaitForSeconds(2f);
        else yield return new WaitForSeconds(Random.Range(1.5f, 3f));

        startReloading();


        for (int i = 0; i < data.bulletsPerSalvo; i++)
        {
            GameObject M = GameObject.Instantiate(data.missleData.visuals);
            EnemyMissle MC = M.AddComponent<EnemyMissle>();
            MC.Initialize(data.missleData);

            M.transform.position = transform.position;
            M.transform.rotation = transform.rotation;

            Vector3 spread = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            M.GetComponent<Rigidbody>().AddForce(transform.forward * data.ejectSpeed + spread, ForceMode.Impulse);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
