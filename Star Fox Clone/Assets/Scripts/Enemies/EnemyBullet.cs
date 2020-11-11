using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 25;
    public float lifetime = 6f;
    private float lifetimeEnd;
    public void Initialize(float _damage, float _lifetime)
    {
        damage = _damage;
        lifetime = _lifetime;
        Start();
    }
    private void Start()
    {
        lifetimeEnd = Time.time + lifetime;
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
        if (other.GetComponent<Player>() != null)
        {
            other.GetComponent<Player>().takeDamage(damage);
        }
        OnHit();
    }

    void OnHit()
    {
        Destroy(gameObject);
    }
}
