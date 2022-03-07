using ProtocFiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFactory : MonoBehaviour
{
    public void CreateTurret(TurretData data, Transform turretParent)
    {
        #region set Mesh
        GameObject T = Instantiate(data.TurretMesh);
        T.transform.parent = turretParent;
        T.transform.position = turretParent.position;
        T.transform.rotation = turretParent.rotation;
        Destroy(T.GetComponent<Turret>());
        #endregion

        #region set Script
        Turret TurretScript;

        switch (data.turretType)
        {
            case TurretClass_P.Ams:
                TurretScript = T.AddComponent<AMSTurret>();
                TurretScript.Data = data;
                break;
            case TurretClass_P.Atg:
                TurretScript = T.AddComponent<ATGTurret>();
                TurretScript.Data = data;
                break;
            case TurretClass_P.Msl:
                TurretScript = T.AddComponent<MSLTurret>();
                TurretScript.Data = data;
                break;
        }
        #endregion
    }
}
