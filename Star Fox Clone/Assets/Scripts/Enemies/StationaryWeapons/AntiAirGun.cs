using System;
using System.Collections;
using UnityEngine;

public class AntiAirGun : StationaryWeapon
{
    protected float flakDelay;

    /// <summary>
    /// Aims directly for the player and calculates bullet flight time for flak ammunition
    /// </summary>
    protected override void aim()
    {
        Vector3 InterceptPoint = getInterceptPoint();
        if (InterceptPoint.magnitude == 0) return;

        flakDelay = Vector3.Distance(transform.position, InterceptPoint) * data.bulletData.speed;

        HelperFunctions.LookAt(transform, InterceptPoint, data.turretSpeed, RotationParent.up);
    }

    protected override IEnumerator Fire()
    {
        startReloading();
        //waiting for the turret to turn
        yield return new WaitForSeconds(2f);
        placeMarker(getInterceptPoint());


        for (int i = 0; i < data.bulletsPerSalvo; i++)
        {
            GameObject b = GameObject.Instantiate(data.bulletData.visuals);
            b.transform.position = transform.position;
            b.transform.rotation = transform.rotation;
            b.layer = 11;

            Bullet bullet = b.AddComponent<Bullet>();

            if(data.bulletData.damageType == DamageType.flak) bullet.Initialize(data.bulletData, data.bulletspread, BulletOrigin.Enemy, MyVelocity, flakDelay);
            else bullet.Initialize(data.bulletData, data.bulletspread, BulletOrigin.Enemy, MyVelocity);
            

            yield return new WaitForSeconds(data.ejectSpeed);
        }
    }
}
