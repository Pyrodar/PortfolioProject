using UnityEngine;

public interface IVehicle
{
    void takeDamage(float damage, DamageType damageType);

    void destroySelf();
}

#region enums
public enum DamageType
{
    basic,
    armourpiercing,
    highExplosive,
    flak,
    collision,
    repairs
}

public enum TargetType
{
    vehicle
    , missle
    , plane
}
#endregion

[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour , IVehicle
{
    protected TargetType type;
    public TargetType Type { get {return type; } }

    [SerializeField] protected float maxHealth = 1;
    protected float currentHealth;
    [SerializeField] protected float armour = 0;
    public float Armour
    {
        get { return armour; }
    }
    [SerializeField] protected float collisionDamage = 25;
    public float Damage
    {
        get { return collisionDamage; }
    }
    protected Player myTarget;

    protected Rigidbody rigid;

    protected virtual void Start()
    {
        myTarget = GameStateConnection.Instance.getFrontlinePlayer();
        currentHealth = maxHealth;
        rigid = GetComponent<Rigidbody>();

        GameStateConnection.Instance.switchingPlayers += changeTarget;
        //Debug.Log("Added object: " + name + " to list of switch player delegate");
    }

    protected void changeTarget()
    {
        myTarget = GameStateConnection.Instance.getFrontlinePlayer();
    }

    public virtual void destroySelf()
    {
        GameStateConnection.Instance.switchingPlayers -= changeTarget;
        //Debug.Log("Removed object: " + name + " to list of switch player delegate");
        foreach (StationaryWeapon SW in GetComponentsInChildren<StationaryWeapon>())
        {
            SW.destroySelf();
        }

        changeTarget();
        if (type != TargetType.missle) myTarget.removeMarkedTarget(this);
        Destroy(this.gameObject);
    }

    public void takeDamage(float dmg)
    {
        float ptrDmg = Mathf.Clamp(dmg - armour, 0, 500);
        if (currentHealth <= 0) return;
        currentHealth -= ptrDmg;
        if (currentHealth <= 0) destroySelf();
    }

    public void takeDamage(float dmg, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.armourpiercing:
                takeDamage(dmg + armour);
                break;

            default:
                takeDamage(dmg);
                break;
        }
    }

    public Vector3 getVelocity()
    {
        return rigid.velocity;
    }
    
    public Vector3 getAVelocity()
    {
        return rigid.angularVelocity;
    }
}
