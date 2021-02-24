using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    #region Singleton
    public static Server Instance;
    public void Awake()
    {
        if (Server.Instance != null)
        {
            Debug.LogError("More than one Server exists!");
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region PlaneSynchronisation
    public void SynchronizePlanePosition(float ditanceTraveled)
    {

    }
    #endregion

    #region Players
    public void UpdatePlayerPosition(Player player)
    {

    }

    public void PlayerConnected(Player player)
    {
        
    }

    public void PlayerDestroyed(Player player)
    {
        
    }
    #endregion

    #region Targets
    public void TargetCreated(Target target)
    {

    }

    public void TargetDestroyed(Target target)
    {

    }
    #endregion

    #region bullets
    public void SpawnBullet(BulletData data, Vector3 spawnlocation, Vector3 bulletVelocity, BulletOrigin origin)
    {

    }
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
