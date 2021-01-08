using UnityEngine;

public class EnemyMissle : Target
{
    MissleData data;
    private float timeWhenArmed = 0;

    bool lostTarget = false;

    public void Initialize(MissleData data)
    {
        this.data = data;
        collisionDamage = data.damage;
        rigid = GetComponent<Rigidbody>();
        rigid.drag = data.drag; 
    }

    protected override void Start()
    {
        base.Start();
        type = TargetType.missle;
        myTarget.addIncomingMissle(this);
        timeWhenArmed = Time.time + data.timeBeforeArmed;
    }

    private void Update()
    {
        if (lostTarget) return;

        addForwardMomentum();

        if (isInRange()) followPlayer();

        if (isBehindPlayer()) looseTarget();
    }

    void addForwardMomentum()
    {
        rigid.AddForce(transform.TransformDirection(Vector3.forward) * data.speed);
    }

    void followPlayer()
    {
        //calculating missle course with double player Velocity for better tracking
        Vector3 interceptCourse = HelperFunctions.Intercept(transform.position, getVelocity(), data.speed, myTarget.transform.position, myTarget.getVelocity() * 2);
        HelperFunctions.LookAt(transform, interceptCourse, data.turnSpeed);
    }

    private bool isBehindPlayer()
    {
        if (myTarget.Plane.relativeZposition(this.transform.position) < 1) return true;
        return false;
    }

    void looseTarget()
    {
        lostTarget = true;
        rigid.useGravity = true;
        Invoke("destroySelf", 2f);
    }

    bool isInRange()
    {
        if(Vector3.Distance(transform.position, myTarget.transform.position) > data.detectionRange || Time.time < timeWhenArmed) return false;
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy" && other.tag != "PlayerBullet")
        {
            Debug.Log("Missle hit something");
            //slowed down detonation to retain the impact of the missle
            Invoke("detonate", 0.05f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Enemy")
        {
            Debug.Log("Missle collided with something");
            //slowed down detonation to retain the impact of the missle
            Invoke("detonate", 0.05f);
        }
    }

    private void detonate()
    {
        Collider[] Colliders = HelperFunctions.SpawnExplosion(data.explosionVisuals, 1, transform.position);

        foreach (Collider hit in Colliders)
        {
            IVehicle t = hit.GetComponent<IVehicle>();
            if (t != null)
            {
                t.takeDamage(data.damage);
            }
        }

        destroySelf();
    }

    public override void destroySelf()
    {
        myTarget.removeIncomingMissle(this);
        base.destroySelf();
    }
}
