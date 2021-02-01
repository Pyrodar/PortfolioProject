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
    #region Type
    protected TargetType type;
    public TargetType Type { get {return type; } }
    #endregion

    #region Health
    [SerializeField] protected float maxHealth = 1;
    protected float currentHealth;

    [SerializeField] protected float armour = 0;
    //public float Armour{get { return armour; }}

    [SerializeField] protected float collisionDamage = 25;
    public float Damage{get { return collisionDamage; }}
    #endregion

    #region rigidbody
    protected Rigidbody rigid;
    public Vector3 Velocity { get { return rigid.velocity; } }
    public Vector3 AVelocity { get { return rigid.angularVelocity; } }
    #endregion

    protected Player myTarget;
    protected Player[] Players { get { return GameStateConnection.Instance.Players; } }


    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rigid = GetComponent<Rigidbody>();

        if (GameStateConnection.Instance != null)
        {
            GameStateConnection.Instance.switchingPlayers += changeTarget;

            changeTarget();
        }
    }

    protected void changeTarget()
    {
        if (GameStateConnection.Instance == null)
        {
            myTarget = null;
            return;
        }
        myTarget = GameStateConnection.Instance.getFrontlinePlayer();
    }

    protected virtual void addScoreAndDestroy()
    {
        //TODO: Add destruction to score based on "TargetType"
        destroySelf();
    }

    public virtual void destroySelf()
    {
        GameStateConnection.Instance.switchingPlayers -= changeTarget;

        foreach (StationaryWeapon SW in GetComponentsInChildren<StationaryWeapon>())
        {
            SW.destroySelf();
        }

        changeTarget();
        if (type != TargetType.missle)
        {
            foreach (var player in Players)
            {
                player.removeMarkedTarget(this);
            }

        }
        Destroy(this.gameObject);
    }

    public void takeDamage(float dmg)
    {
        if (currentHealth <= 0) return;

        float ptrDmg = Mathf.Clamp(dmg - armour, 0, 500);
        currentHealth -= ptrDmg;
        
        if (currentHealth <= 0) addScoreAndDestroy();
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
}
