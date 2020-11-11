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
        myTarget = Player.Instance;
        currentHealth = maxHealth;
        rigid = GetComponent<Rigidbody>();
    }

    public virtual void destroySelf()
    {
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
