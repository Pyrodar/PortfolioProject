using UnityEngine;

public class PlayerMissle : MonoBehaviour
{
    private MissleData data;
    private float timeWhenArmed = 0;

    public AquiredTarget target;
    bool lostTarget = false;
    float lastDistance = 100000;

    Rigidbody myRigid;

    public void Initialize(AquiredTarget _target, MissleData _data)
    {
        target = _target;
        data = _data;
        myRigid = GetComponent<Rigidbody>();
        myRigid.drag = _data.drag;
    }

    private void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        timeWhenArmed = Time.time + data.timeBeforeArmed;
    }

    private void Update()
    {
        if (lostTarget) return;
        if (target.transform == null)
        {
            looseTarget();
            return;
        }

        followTarget();
    }

    void followTarget()
    {
        if (Time.time < timeWhenArmed) 
        {
            //Starts slowly homing in on the Target
            HelperFunctions.LookAt(transform, target.transform.position, data.turnSpeed / 5);
            return;
        }

        //Starts homing in on the Target with regular speed
        Vector3 IntercepCourse = HelperFunctions.Intercept(transform.position, myRigid.velocity, data.speed, target.transform.position, target.Velocity);
        HelperFunctions.LookAt(transform, IntercepCourse, data.turnSpeed);

        //forward Momentum
        myRigid.AddForce(transform.TransformDirection(Vector3.forward) * data.speed);
    }

    //Looses Target after missing it
    private void LateUpdate()
    {
        //adding two seconds to the timer to gain some speed first
        if (Time.time < timeWhenArmed + 2f || lostTarget) return;
        float currentDistance = Vector3.Distance(transform.position, target.transform.position);

        //Ignore if target is still far away
        if (currentDistance > 20) return;

        if (currentDistance > lastDistance) looseTarget();
        else lastDistance = currentDistance;
    }

    void looseTarget()
    {
        lostTarget = true;
        myRigid.useGravity = true;
        Invoke("getDestroyed", 2f);
    }

    void getDestroyed()
    {
        Destroy(gameObject);
    }

    public Vector3 getVelocity()
    {
        return myRigid.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit();
    }

    void OnHit()
    {
        Collider[] expHhit = HelperFunctions.SpawnExplosion(data.explosionVisuals, data.explosionRadius, transform.position);
        foreach (var other in expHhit)
        {
            if (other.GetComponent<Target>() != null)
            {
                other.GetComponent<Target>().takeDamage(data.damage, data.damageType);
            }
        }

        Destroy(gameObject);
    }
}
