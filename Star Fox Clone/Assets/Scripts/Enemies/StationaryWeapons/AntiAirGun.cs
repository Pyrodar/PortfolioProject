using System;
using System.Collections;
using UnityEngine;

public class AntiAirGun : StationaryWeapon
{
    protected override Vector3 getInterceptPoint()
    {
        return HelperFunctions.Intercept(transform.position, Vector3.zero, data.bulletData.speed, myTarget.transform.position, myTarget.getVelocity());
    }

    protected override IEnumerator Fire()
    {
        startReloading();
        //waiting for the turret to turn
        yield return new WaitForSeconds(2f);
        placeMarker(getInterceptPoint());

        //Debug.Log("Fire!");

        for (int i = 0; i < data.bulletsPerSalvo; i++)
        {
            GameObject b = GameObject.Instantiate(data.bulletData.visuals);
            b.transform.position = transform.position;
            b.transform.rotation = transform.rotation;
            b.layer = 11;

            Bullet bullet = b.AddComponent<Bullet>();
            bullet.Initialize(data.bulletData, data.bulletspread);

            yield return new WaitForSeconds(data.ejectSpeed);
        }
    }
}
