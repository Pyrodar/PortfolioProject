using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetPlayer : MonoBehaviour , IVehicle
{
    TurretMount[] turretMounts;

    public PuppetPlayer(TurretMount[] mounts)
    {
        turretMounts = mounts;
    }

    public void UpdatePosition()
    {

    }
    public void UpdateRotation()
    {

    }
    public void UpdateTurrets()
    {

    }

    #region Interface

    public void destroySelf()
    {
        throw new System.NotImplementedException();
    }

    public void takeDamage(float damage, DamageType damageType)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
