using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairField : MonoBehaviour
{
    [SerializeField] float repairSpeed;
    IVehicle objectToRepair;

    private void OnTriggerEnter(Collider other)
    {
        IVehicle obj = other.gameObject.GetComponent<IVehicle>();
        if (obj != null)
        {
            objectToRepair = obj;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IVehicle obj = other.gameObject.GetComponent<IVehicle>();
        if (obj == objectToRepair)
        {
            objectToRepair = null;
        }
    }

    private void Update()
    {
        if(objectToRepair != null)objectToRepair.takeDamage(repairSpeed * Time.deltaTime, DamageType.repairs);
    }
}
