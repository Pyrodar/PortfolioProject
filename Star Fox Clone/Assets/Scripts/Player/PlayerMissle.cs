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
        //Starts homing in on the Target
        Vector3 IntercepCourse = HelperFunctions.Intercept(transform.position, myRigid.velocity, data.speed, target.transform.position, target.velocity);
        HelperFunctions.LookAt(transform, IntercepCourse, data.turnSpeed);

        if (Time.time < timeWhenArmed) return;

        //forward Momentum
        myRigid.AddForce(transform.TransformDirection(Vector3.forward) * data.speed);
    }

    //Looses Target after missing it
    private void LateUpdate()
    {
        //adding two seconds to the timer to gain some speed first
        if (Time.time < timeWhenArmed + 2f || lostTarget) return;
        float currentDistance = Vector3.Distance(transform.position, target.transform.position);
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
        if (other.GetComponent<Target>() != null)
        {
            other.GetComponent<Target>().takeDamage(data.damage);
        }
        OnHit();
    }
    void OnHit()
    {
        Debug.Log("EXPLOSION!");

        Destroy(gameObject);
    }
}
