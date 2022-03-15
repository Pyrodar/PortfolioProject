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

        BulletFactory factory = MapLayoutInfo.Instance.BulletFactory;

        for (int i = 0; i < data.bulletsPerSalvo; i++)
        {
            factory.CmdSpawnBullet(BulletOrigin.Enemy, data.bulletData, transform.position, transform.rotation, data.bulletspread, flakDelay, MyVelocity);

            yield return new WaitForSeconds(data.ejectSpeed);
        }
    }
}
