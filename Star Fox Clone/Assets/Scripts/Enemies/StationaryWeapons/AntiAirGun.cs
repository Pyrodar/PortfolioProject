using System.Collections;
using UnityEngine;

public class AntiAirGun : StationaryWeapon
{
    protected float flakDelay;

    [SerializeField]Vector3 interceptPoint;

    protected override void Update()
    {
        if (!isActive) return;

        if (isInRange())
        {
            aim();

            if (isLoaded() && (InSights(interceptPoint) || synchronized)) StartCoroutine(Fire());
        }
    }

    /// <summary>
    /// Aims directly for the player and calculates bullet flight time for flak ammunition
    /// </summary>
    protected override void aim()
    {
        interceptPoint = getInterceptPoint();
        if (interceptPoint.magnitude == 0) return;

        flakDelay = Vector3.Distance(transform.position, interceptPoint) * data.bulletData.speed;

        HelperFunctions.LookAt(transform, interceptPoint, data.turretSpeed, RotationParent.up);
    }

    protected override IEnumerator Fire()
    {
        startReloading();
        placeMarker(interceptPoint);

        BulletFactory factory = MapLayoutInfo.Instance.BulletFactory;

        for (int i = 0; i < data.bulletsPerSalvo; i++)
        {
            factory.CmdSpawnBullet(BulletOrigin.Enemy, data.bulletData, transform.position, transform.rotation, data.bulletspread, flakDelay, MyVelocity);

            yield return new WaitForSeconds(data.ejectSpeed);
        }

    }
}
