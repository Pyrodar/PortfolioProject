using UnityEngine;

public class PlayerMissle : MonoBehaviour, IVehicle
{
    private MissleData data;
    private float hitPoints;
    private float timeWhenArmed = 0;

    public AquiredTarget target;
    bool lostTarget = false;
    float lastDistance = 100000;

    Rigidbody myRigid;
    public Vector3 Velocity
    {
        get { return myRigid.velocity; }
    }


    public void Initialize(AquiredTarget _target, MissleData _data)
    {
        target = _target;
        data = _data;
        myRigid = GetComponent<Rigidbody>();
        myRigid.drag = _data.drag;

        hitPoints = data.hitpoints * 3;                             //Playermissles get more health so they don't get shot down too easy
    }

    private void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        timeWhenArmed = Time.time + data.timeBeforeArmed;
    }

    private void FixedUpdate()
    {
        if (lostTarget) return;
        if (target.transform == null)
        {
            looseTarget();
            return;
        }

        followTarget();

        LooseTarget();
    }

    #region followTarget
    void followTarget()
    {
        if (Time.time < timeWhenArmed) 
        {
            HelperFunctions.LookAt(transform, target.transform.position, data.turnSpeed / 10);                                                               //Starts slowly homing in on the Target
            return;
        }

        Vector3 IntercepCourse = HelperFunctions.Intercept(transform.position, myRigid.velocity, data.speed, target.transform.position, target.Velocity);   //Starts homing in on the Target with regular speed
        HelperFunctions.LookAt(transform, IntercepCourse, data.turnSpeed);

        myRigid.AddForce(transform.TransformDirection(Vector3.forward) * data.speed);                                                                       //Constant forward Momentum
    }

    private void LooseTarget()                                                                           //Looses Target after missing it
    {
        if (Time.time < timeWhenArmed + 2f || lostTarget) return;                                       //always adding two seconds to the timer to gain some speed first
        float currentDistance = Vector3.Distance(transform.position, target.transform.position);

        if (currentDistance > 20) return;                                                               //Ignore if target is still far away

        if (currentDistance > lastDistance +.1f) looseTarget();
        else lastDistance = currentDistance;
    }

    void looseTarget()
    {
        lostTarget = true;
        myRigid.useGravity = true;
        Invoke("getDestroyed", 3f);                                 //Missles despawn after falling for 3 seconds in case they fall through the floor
    }
    #endregion

    #region explosion and destruction
    private void OnTriggerEnter(Collider other)
    {
        detonate();
    }

    public void takeDamage(float damage, DamageType damageType)
    {
        hitPoints -= damage;
        if (hitPoints <= 0) detonate();                            //Playermissles explode upon destruction to better indicate that they were shot down
    }

    void detonate()
    {
        Collider[] expHhit = HelperFunctions.SpawnExplosion(data.explosionVisuals, data.explosionRadius, transform.position);
        foreach (var other in expHhit)
        {
            if (other.GetComponent<Target>() != null)
            {
                other.GetComponent<Target>().takeDamage(data.damage, data.damageType);
            }
        }

        destroySelf();
    }

    public void destroySelf()
    {
        Destroy(gameObject);
    }
    #endregion
}
