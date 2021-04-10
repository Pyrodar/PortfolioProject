using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ServerInterface
{
    #region PlaneSynchronisation
    void SynchronizePlanePosition(float distanceTraveled);

    #endregion

    #region Players
    void UpdatePlayerPosition(Player player);


    void PlayerConnected(Player player);


    void PlayerDestroyed(Player player);

    #endregion

    #region Targets
    void TargetCreated(Target target);

    void TargetDestroyed(Target target);
    #endregion

    #region bullets
    void SpawnBullet(BulletData data, Vector3 spawnlocation, Vector3 bulletVelocity, BulletOrigin origin);
    #endregion
}

#region structs

public struct PlayerInfo
{
    #region Position
    float PosX;
    float PosY;
    float PosZ;
    #endregion

    #region Rotation Euler
    float RotX;
    float RotY;
    float RotZ;
    #endregion

    #region Turrets
    #endregion
}

public struct TargetInfo
{

}

public struct BulletInfo
{
    #region Data String
    string data;
    #endregion

    #region Position
    float PosX;
    float PosY;
    float PosZ;
    #endregion

    #region Rotation Euler
    float RotX;
    float RotY;
    float RotZ;
    #endregion

    #region Velocity
    float VelX;
    float VelY;
    float VelZ;
    #endregion
}

public struct MissleInfo
{

}

#endregion
