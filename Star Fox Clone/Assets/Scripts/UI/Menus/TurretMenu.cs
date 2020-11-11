using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMenu : MonoBehaviour
{
    [SerializeField] Transform menuObject;
    List<Turret> TurretPrefabs;
    TurretMount selectedMount;

    public void OpenMenu(TurretMount mount)
    {
        selectedMount = mount;

    }
}
