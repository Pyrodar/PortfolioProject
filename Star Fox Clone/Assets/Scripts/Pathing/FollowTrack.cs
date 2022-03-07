using System;
using UnityEngine;
using UnityEngine.Events;
using PathCreation;

/// <summary>
/// This script used a rigidbodys velocity to move so the velocity could be added to the gameobject for targeting purposes.
/// now it calculates its velocity based on its last position, since that allows more control over the current position and rotation
/// </summary>
public class FollowTrack : MonoBehaviour
{
    [SerializeField] float speed;
    public float Speed
    {
        get { return speed; }
    }
    [SerializeField] PathCreator Path;
    float pathPosition = 0;
    Vector3 PathPosition
    {
        get {
            if (!go) return Vector3.zero;
            
            return Path.path.GetPointAtDistance(pathPosition); 
        }
    }

    Vector3 PathNormal
    {
        get
        {
            if (!go) return Vector3.zero;

            return Path.path.GetNormal(pathPosition);
        }
    }
    bool go = false;

    //Rigidbody rigid;

    public UnityEvent trackEnded;

    Vector3 lastPosition = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    public Vector3 Velocity
    {
        get
        {   return velocity; }
    }


    private void Awake()
    {
        //rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //DEBUGGING##################
        if (Input.GetKey(KeyCode.G))
        {
            debugSpeed(24);
        }
        else
        {
            debugSpeed(8);
        }

        //#########################
        
        if (!go) return;

        moveAlongPath();

        calculateVelocity();

        if (IsEndReached())
        {
            StopFollow();
            trackEnded.Invoke();
        }
    }

    void calculateVelocity()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;

        lastPosition = transform.position;
    }

    void moveAlongPath()
    {
        pathPosition += speed * Time.deltaTime;

        transform.position = PathPosition;

        //Debug.Log("Normal : " + PathNormal);

        //transform.LookAt(transform.position + PathNormal);

    }

    public void StartFollow()
    {
        pathPosition = 8;
        go = true;
        gameObject.SetActive(true);
        transform.position = PathPosition;

        //if (rigid != null) rigid.drag = 0f;
    }

    public void StopFollow()
    {
        go = false;
        //rigid.drag = 1f;
    }

    internal int getCurrentWaypoint()
    {
        //TODO: find out how to access last waypoint
        return 0;
    }

    bool IsEndReached()
    {
        return pathPosition >= Path.path.length - 1;
    }

    
    public void debugSpeed(float i)
    {
        speed = i;
    }
}
