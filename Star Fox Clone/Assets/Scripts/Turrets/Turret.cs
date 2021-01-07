using UnityEngine;

public class Turret : MonoBehaviour
{
    protected TurretMount myMount;

    [SerializeField]protected TurretData data;
    public TurretData Data
    {
        //get { return data; }
        set { data = value; }
    }

    public TurretType TurretType
    {
        get { return data.turretType; }
    }

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

    public virtual void Fire()
    {
        //Debug.Log("Firing");
    }
}

