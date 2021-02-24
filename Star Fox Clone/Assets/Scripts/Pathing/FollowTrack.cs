using System;
using UnityEngine;
using UnityEngine.Events;
using PathCreation;

/// <summary>
/// This script uses a rigidbodys velocity to move so the velocity can be added to the gameobject for targeting purposes. 
/// [The actual cart following the path uses the "Cart" script]NO LONGER ACCURATE
/// removed the cart, since only the position on the path was beeing used
/// </summary>
public class FollowTrack : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] PathCreator Path;
    float pathPosition = 0;
    Vector3 PathPosition
    {
        get { return Path.path.GetPointAtDistance(pathPosition); }
    }
    bool go = false;

    Rigidbody rigid;

    public UnityEvent trackEnded;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        transform.position = PathPosition;
        pathPosition = 8;
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

        if (IsEndReached())
        {
            StopFollow();
            trackEnded.Invoke();
        }
    }

    void moveAlongPath()
    {
        rigid.velocity = transform.forward * speed;
        pathPosition += speed * Time.deltaTime;
        transform.LookAt(PathPosition);

        //ensuring cart always has a certain distance to this object
        float distance = Vector3.Distance(transform.position, PathPosition);
        if(distance > 8.5) pathPosition -= speed * Time.deltaTime;
        if(distance < 7.5) pathPosition += speed * Time.deltaTime;
    }

    public void StartFollow()
    {
        go = true;
        gameObject.SetActive(true);

        if (rigid != null) rigid.drag = 0f;
    }

    public void StopFollow()
    {
        go = false;
        rigid.drag = 1f;
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
