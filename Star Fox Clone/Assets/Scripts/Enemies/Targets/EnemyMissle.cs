using UnityEngine;

public class EnemyMissle : Target
{
    MissleData data;
    private float timeWhenArmed = 0;

    bool lostTarget = false;

    Player lockedTarget;

    public void Initialize(MissleData data)
    {
        this.data = data;

        maxHealth = data.hitpoints;
        currentHealth = maxHealth;

        collisionDamage = data.damage;
        rigid = GetComponent<Rigidbody>();
        rigid.drag = data.drag;
    }

    protected override void Start()
    {
        base.Start();
        type = TargetType.missle;
        //myTarget.addIncomingMissle(this);
        foreach (var player in Players)
        {
            player.addIncomingMissle(this);
        }
        timeWhenArmed = Time.time + data.timeBeforeArmed;
    }

    private void FixedUpdate()
    {
        if (lostTarget) return;

        addForwardMomentum();

        if (isInDetectionRange()) followPlayer();

        if (isBehindPlayer()) looseTarget();
    }

    void addForwardMomentum()
    {
        rigid.AddForce(transform.TransformDirection(Vector3.forward) * data.speed);
    }

    void followPlayer()
    {
        if (isInArmingRange())
        {
            //calculating missle course with double player Velocity for better tracking
            Vector3 interceptCourse = HelperFunctions.Intercept(transform.position, Vector3.zero, data.speed, lockedTarget.transform.position, lockedTarget.Velocity * 2);
            HelperFunctions.LookAt(transform, interceptCourse, data.turnSpeed);
        }
        else
        {
            //Aiming Above the player first for better AMS coverage
            Vector3 AMSArea = myTarget.transform.position + new Vector3(0, 5, 0);
            Vector3 interceptCourse = HelperFunctions.Intercept(transform.position, Vector3.zero, data.speed, AMSArea, myTarget.Velocity * 2);
            HelperFunctions.LookAt(transform, interceptCourse, data.turnSpeed);
        }
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

    bool isInDetectionRange()
    {
        if(Vector3.Distance(transform.position, myTarget.transform.position) > data.detectionRange || Time.time < timeWhenArmed) return false;
        return true;
    }
    
    bool isInArmingRange()          //Checks if the target is in range to engage directly and locks the target so switching who's in front doesn't allow for easy dodging
    {
        if(Vector3.Distance(transform.position, myTarget.transform.position) > data.armingRange || Time.time < timeWhenArmed) return false;
        lockedTarget = myTarget;
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy" && other.tag != "AMSBullet")
        {
            //Debug.Log("Missle hit something");
            //delayed detonation to retain the impact of the missle TODO: just add knockback
            Invoke("detonate", 0.05f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Enemy")
        {
            //Debug.Log("Missle collided with something");
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
                t.takeDamage(data.damage, data.damageType);
            }
        }

        destroySelf();
    }

    public override void destroySelf()
    {
        //myTarget.removeIncomingMissle(this);
        foreach (var player in Players)
        {
            player.removeIncomingMissle(this);
        }
        base.destroySelf();
    }
}
