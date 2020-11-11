using System.Collections;
using UnityEngine;

public class MissleLauncher : StationaryWeapon
{
    protected override void aim()
    {
        Vector3 offset = new Vector3(0, 15, 0);

        Vector3 InterceptPoint = getInterceptPoint();
        if (InterceptPoint.magnitude == 0) return;

        HelperFunctions.LookAt(transform, InterceptPoint + offset, data.turretSpeed, RotationParent.up);
    }

    protected override Vector3 getInterceptPoint()
    {
        return HelperFunctions.Intercept(transform.position, Vector3.zero, data.missleData.speed, myTarget.transform.position, myTarget.getVelocity());
    }

    protected override IEnumerator Fire()
    {
        startReloading();
        //waiting for the turret to turn
        yield return new WaitForSeconds(2f);

        //Debug.Log("Fire!");

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
