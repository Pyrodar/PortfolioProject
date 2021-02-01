using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletData data;
    private float lifetimeEnd;

    public void Initialize(BulletData _data, float bulletSpread, BulletOrigin origin)
    {
        data = _data;
        lifetimeEnd = Time.time + data.lifetime;
        Rigidbody r = GetComponent<Rigidbody>();
        switch (origin)
        {
            case BulletOrigin.Player:
                gameObject.layer = 12; //Player
                break;
            default:
                gameObject.layer = 11; //Enemies
                break;
        }

        Vector3 spread = new Vector3(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread));
        r.AddForce(transform.forward * data.speed + spread, ForceMode.Impulse);
    }

    public void SetFlakTime(float time)
    {
        lifetimeEnd = Time.time + time;
    }

    private void Update()
    {
        if (Time.time > lifetimeEnd)
        {
            destruction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "AMSBullet") OnHit(other);
    }

    private void dealDamage(Collider other)
    {
        if (other.GetComponent<IVehicle>() != null)
        {
            other.GetComponent<IVehicle>().takeDamage(data.damage, data.damageType);
        }
    }

    void OnHit(Collider other)
    {
        if(data.damageType == DamageType.highExplosive) Explode();
        else dealDamage(other);
        
        Destroy(gameObject);
    }

    void Explode()
    {
        //Spawn Particles and get Colliders in detonation radius
        Collider[] Colliders = HelperFunctions.SpawnExplosion(data.explosionVisuals, data.radius, transform.position);

        foreach (Collider hit in Colliders)
        {
            IVehicle t = hit.GetComponent<IVehicle>();
            if (t != null)
            {
                t.takeDamage(data.damage, data.damageType);
            }
        }
    }

    void destruction()
    {
        if (data.damageType == DamageType.flak)
        {
            Explode();
        }
        Destroy(this.gameObject);
    }
}

public enum BulletOrigin
{
    Player
    ,Enemy
}
