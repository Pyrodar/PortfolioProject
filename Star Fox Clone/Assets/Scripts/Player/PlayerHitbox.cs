using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour , IVehicle
{
    [SerializeField] Player player;

    [SerializeField] float collisionDamage = 25;

    /// <summary>
    /// Checks for collisions with the environment
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Environment")
        {
            Debug.Log("scratched the environment for " + collisionDamage + " damage");
            takeDamage(collisionDamage);
        }
    }

    public void takeDamage(float damage)
    {
        player.takeDamage(damage);
    }

    public void destroySelf()
    {
        //only exists for IVehicle
    }
}
