using System;
using System.Collections;
using UnityEngine;

public class AntiAirGun : StationaryWeapon
{
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
            bullet.Initialize(data.bulletData, data.bulletspread, BulletOrigin.Enemy, MyVelocity);

            yield return new WaitForSeconds(data.ejectSpeed);
        }
    }
}
