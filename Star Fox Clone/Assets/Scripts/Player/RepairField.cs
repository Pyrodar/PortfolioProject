using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairField : MonoBehaviour
{
    [SerializeField] float repairSpeed;
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("REPAIRING?");
        IVehicle player = collision.gameObject.GetComponent<IVehicle>();
        if (player != null)
        {
            player.takeDamage(repairSpeed * Time.deltaTime, DamageType.repairs);
        }
    }
}
