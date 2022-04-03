using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoidBase : MonoBehaviour
{
    protected List<BoidBase> boidBases = new List<BoidBase>();
    protected Rigidbody myRigid;
    protected Transform target;

    //move into scriptedobject #############
    protected float speed = 10f;
    protected float rotationSpeed = 12f;
    protected float lockOnDistance;
    //######################################

    protected float VicinityWeight = 1f;
    protected float TargetWeight = 1f;
    protected float EvasionWeight = 1f;


    public Vector3 BoidDirection
    {
        get { return transform.forward; }
    }

    public BoidBase()
    {

    }

    void Initialize(Transform newTarget, List<BoidBase> boidsInTeam, Vector3 launchForce)
    {
        myRigid = GetComponent<Rigidbody>();

        boidBases.Clear();
        boidBases = boidsInTeam;

        myRigid.AddForce(launchForce, ForceMode.Impulse);


        //starts the followingProcess
        target = newTarget;
    }

    void FixedUpdate()
    {
        if (!target) return;

        HelperFunctions.LookAt(transform, getNextDirection(), rotationSpeed);

        applyForwardMomentum();
    }

    //get the direction to take based on the different vectors 
    Vector3 getNextDirection()
    {
        Vector3 nextDir = Vector3.zero;



        nextDir += getVicinityBoidsDirection() * VicinityWeight; 

        nextDir += getTargetPosition() * TargetWeight;                                  //increase weight when close to the target

        if (Vector3.Magnitude(target.position - transform.position) < lockOnDistance)   //Only checks for collisions when too far away from the target to detonate
        {
            nextDir += getEvasiveManeuver() * EvasionWeight;
        }



        nextDir = nextDir.normalized;
        return nextDir;
    }

    //GEt the average direction other boids in the group are moving
    Vector3 getVicinityBoidsDirection()
    {
        Vector3 averageDir = Vector3.zero;

        foreach (BoidBase boidBase in boidBases)
        {
            averageDir += boidBase.BoidDirection;
        }

        averageDir = averageDir.normalized;

        return averageDir;
    }

    Vector3 getTargetPosition()
    {
        return (target.position - transform.position).normalized;
    }

    Vector3 getEvasiveManeuver()
    {
        int amountOfEvasivedirections = 10;

        for (int i = 0; i < amountOfEvasivedirections; i++)
        {
            Vector3 direction = Vector3.zero;   //calculate rotatioary sequenz of directions to check
            if (isPathFree(direction))
            {
                return direction.normalized;    //return first free direction
            }
        }

        return Vector3.zero;
    }

    bool isPathFree(Vector3 direction)
    {
        //check direction via raycast

        float distanceChecked = 2f;

        Collider c = HelperFunctions.GetObjectInSights(transform.position, transform.position + direction, distanceChecked);

        EvasionWeight = 1f; //evasionweight will be changed based on the distance to the object to be evaded

        return true;
    }

    void applyForwardMomentum()
    {
        myRigid.AddForce(transform.forward * speed);
    }
}
