using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Intended to create a base class for Missles but realized EnemyMissle already inherits from Target, which makes more sense
public class Missle : MonoBehaviour, IVehicle
{
    private MissleData data;
    private float timeWhenArmed = 0;

    public AquiredTarget target;
    bool lostTarget = false;
    float lastDistance = 100000;

    Rigidbody myRigid;

    public void Initialize(AquiredTarget _target, MissleData _data)
    {
        target = _target;
        data = _data;
        myRigid = GetComponent<Rigidbody>();
        myRigid.drag = _data.drag;
    }

    private void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        timeWhenArmed = Time.time + data.timeBeforeArmed;
    }

    void getDestroyed()
    {
        Destroy(gameObject);
    }

    public Vector3 getVelocity()
    {
        return myRigid.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit();
    }

    void OnHit()
    {
        Collider[] expHhit = HelperFunctions.SpawnExplosion(data.explosionVisuals, data.explosionRadius, transform.position);
        foreach (var other in expHhit)
        {
            if (other.GetComponent<Target>() != null)
            {
                other.GetComponent<Target>().takeDamage(data.damage, data.damageType);
            }
        }

        Destroy(gameObject);
    }

    public void takeDamage(float damage, DamageType damageType)
    {
        throw new System.NotImplementedException();
    }

    public void destroySelf()
    {
        throw new System.NotImplementedException();
    }
}*/
