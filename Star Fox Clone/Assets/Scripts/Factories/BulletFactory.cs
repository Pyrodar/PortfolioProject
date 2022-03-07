using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// will be used to spawn all Bullets underneath the map and teleport them to the muzzle to reduce spawning
/// </summary>
public class BulletFactory : MonoBehaviour
{
    [SerializeField] Transform bulletStorageArea;

    /// <summary>
    /// Spawns a number of bullets for the existing turrets
    /// </summary>
    /// <param name="data"></param>
    public void PrepareBulletsForWeapon(TurretData data)
    {

    }

    public void CmdSpawnBullet(BulletOrigin owner, BulletData data, Vector3 position, Quaternion rotation)
    {
        CmdSpawnBullet(owner, data, position, rotation, 0, 0, Vector3.zero);
    }

    public void CmdSpawnBullet(BulletOrigin owner, BulletData data, Vector3 position, Quaternion rotation, float bulletSpread)
    {
        CmdSpawnBullet(owner, data, position, rotation, bulletSpread, 0, Vector3.zero);
    }

    public void CmdSpawnBullet(BulletOrigin owner, BulletData data, Vector3 position, Quaternion rotation, float bulletSpread, float flakDelay)
    {
        CmdSpawnBullet(owner, data, position, rotation, bulletSpread, flakDelay, Vector3.zero);
    }


    public void CmdSpawnBullet(BulletOrigin owner, BulletData data, Vector3 position, Quaternion rotation, float bulletSpread, float flakDelay, Vector3 parentSpeed)
    {
        GameObject b = Instantiate(data.visuals);
        b.transform.position = position;
        b.transform.rotation = rotation;

        Bullet bullet = b.AddComponent<Bullet>();

        switch (owner)
        {
            case BulletOrigin.Player:
                b.gameObject.layer = 12; //Player
                break;
            case BulletOrigin.AMS:
                b.gameObject.layer = 12; //AMS
                b.gameObject.tag = "AMSBullet";
                break;
            default:
                b.gameObject.layer = 11; //Enemies
                break;
        }

        if (data.damageType == DamageType.flak)
        {
            bullet.Initialize(data, bulletSpread, parentSpeed, flakDelay);    //setting bullet timer manually for flak ammunition
        }
        else bullet.Initialize(data, bulletSpread,parentSpeed);              //using regular bullet timer

        //Spawn bullet in Network
    }

    public void CmdSpawnMissle(MissleData data, Vector3 position, Quaternion rotation, Vector3 ParentSpeed, float ejectSpeed, AquiredTarget target)
    {
        GameObject M = Instantiate(data.visuals);
        PlayerMissle PM = M.AddComponent<PlayerMissle>();
        PM.Initialize(target, data);
        gameObject.layer = 12;

        M.transform.position = position;
        M.transform.rotation = rotation;


        float spreadF = ejectSpeed / 8;
        Vector3 spread = new Vector3(Random.Range(-spreadF, spreadF), Random.Range(-spreadF, spreadF), Random.Range(-spreadF, spreadF));

        M.GetComponent<Rigidbody>().AddForce(M.transform.forward * ejectSpeed + spread + ParentSpeed, ForceMode.Impulse);
    }

    public void CmdSpawnEnemyMissle(MissleData data, Vector3 position, Quaternion rotation, Vector3 ParentSpeed, float ejectSpeed)
    {
        GameObject M = Instantiate(data.visuals);
        EnemyMissle MC = M.AddComponent<EnemyMissle>();
        MC.Initialize(data);
        gameObject.layer = 11;

        M.transform.position = position;
        M.transform.rotation = rotation;


        float spreadF = ejectSpeed / 8;
        Vector3 spread = new Vector3(Random.Range(-spreadF, spreadF), Random.Range(-spreadF, spreadF), Random.Range(-spreadF, spreadF));

        M.GetComponent<Rigidbody>().AddForce(M.transform.forward * ejectSpeed + spread + ParentSpeed, ForceMode.Impulse);
    }

    public void CmdSpawnBomb(BombData data, Vector3 position, Quaternion rotation, Vector3 ParentSpeed)
    {
        GameObject b = Instantiate(data.visuals);
        b.transform.position = position;
        b.transform.rotation = rotation;
        b.layer = 11;

        EnemyBomb bomb = b.AddComponent<EnemyBomb>();
        bomb.Initialize(data);
        b.GetComponent<Rigidbody>().AddForce(ParentSpeed, ForceMode.Impulse);
    }

}
