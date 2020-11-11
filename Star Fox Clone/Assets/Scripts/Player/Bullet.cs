using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletData data;
    private float lifetimeEnd;

    public void Initialize(BulletData _data)
    {
        data = _data;
        lifetimeEnd = Time.time + data.lifetime;
        Rigidbody r = GetComponent<Rigidbody>();
        r.AddForce(transform.forward * data.speed, ForceMode.Impulse);
    }

    private void Update()
    {
        if (Time.time > lifetimeEnd)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit(other);
    }

    private void dealDamage(Collider other)
    {
        if (other.GetComponent<IVehicle>() != null)
        {
            other.GetComponent<IVehicle>().takeDamage(data.damage);
        }
    }

    void OnHit(Collider other)
    {
        if(data.explosive) Explode();
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
                t.takeDamage(data.damage);
            }
        }
    }
}
