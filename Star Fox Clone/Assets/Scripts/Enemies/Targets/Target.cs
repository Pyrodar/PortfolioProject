using UnityEngine;

public interface IVehicle
{
    void takeDamage(float damage);

    void destroySelf();
}


[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour , IVehicle
{
    public TargetType type;

    [SerializeField] protected float maxHealth = 1;
    protected float currentHealth;
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

    public void takeDamage(float dmg = 50)
    {
        //Debug.Log("Taken " + dmg + " damage");
        if (currentHealth <= 0) return;
        currentHealth -= dmg;
        if (currentHealth <= 0) destroySelf();
    }

    public Vector3 getVelocity()
    {
        return rigid.velocity;
    }
}
