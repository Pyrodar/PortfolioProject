using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour, ServerInterface
{
    #region Players
    public void PlayerConnected(Player player)
    {
        throw new System.NotImplementedException();
    }

    public void PlayerDestroyed(Player player)
    {
        throw new System.NotImplementedException();
    }

    public void UpdatePlayerPosition(Player player)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Plane
    public void SynchronizePlanePosition(float distanceTraveled)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Targets
    public void TargetCreated(Target target)
    {
        throw new System.NotImplementedException();
    }

    public void TargetDestroyed(Target target)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Bullets
    public void SpawnBullet(BulletData data, Vector3 spawnlocation, Vector3 bulletVelocity, BulletOrigin origin)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
