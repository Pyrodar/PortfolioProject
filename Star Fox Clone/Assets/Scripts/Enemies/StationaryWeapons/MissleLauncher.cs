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

        Vector3 interceptPoint = getInterceptPoint();
        if (interceptPoint.magnitude == 0) return;

        HelperFunctions.LookAt(transform, interceptPoint + offset, data.turretSpeed, RotationParent.up);
    }

    /// <summary>
    /// uses Missle speed
    /// </summary>
    protected override Vector3 getInterceptPoint()
    {
        return HelperFunctions.Intercept(transform.position, Vector3.zero, data.missleData.speed, myTarget.transform.position, myTarget.Velocity);
    }

    protected override IEnumerator Fire()
    {
        startReloading();       //Has to be set to reload immidiately to avoid calling 2 or more instances of this Coroutine


        //checking for free LOS

        Vector3 ic = getInterceptPoint();
        float dist = Vector3.Distance(transform.position, ic) * 0.75f;  //Reducing range to avoid being blocked by objects around the plane
        Collider c = HelperFunctions.GetObjectInSights(transform.position, ic, dist);

        if (c == null)
        {
            skipLoading();      //Immidiately ready to fire again
            yield return null;
        }
        else
        {
            //waiting for the turret to turn. randomizing it to desynchronize enemies
            if (synchronized) yield return new WaitForSeconds(2f);

            else yield return new WaitForSeconds(Random.Range(1.5f, 3f));

            for (int i = 0; i < data.bulletsPerSalvo; i++)
            {
                BulletFactory factory = MapLayoutInfo.Instance.BulletFactory;
                factory.CmdSpawnEnemyMissle(data.missleData, transform.position, transform.rotation, MyVelocity, data.ejectSpeed);

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}