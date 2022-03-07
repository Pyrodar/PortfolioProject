using ProtocFiles;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField]protected TurretMount myMount;

    [SerializeField]protected TurretData data;

    public TurretData Data
    {
        get { return data; }
        set { data = value; }
    }

    public TurretClass_P TurretType { get { return data.turretType; } }


    protected Player myPlayer { get { return myMount.PlayerReferenz; } }
    protected TurretIcon myHudIcon;
    protected float cooldownEnd = 0f;

    private void Awake()
    {
        myMount = transform.parent.GetComponent<TurretMount>();
        if (myMount != null) myMount.SetTurret(this);
        else Debug.LogError("Turret " + this.name + " has no Mount");
    }

    public void LookAt(Vector3 target)
    {
        HelperFunctions.LookAt(transform, target, data.turretSpeed, myMount.transform.up);
    }
    /// <summary>
    /// Will be overwritten for each turret subtype
    /// </summary>
    public virtual void Fire()
    {
        //Debug.Log("Firing");
    }

    public virtual void ConnectToUI(TurretIcon icon)
    {
        myHudIcon = icon;
        //Debug.Log($"Connected {name} to UI");
    }
}

