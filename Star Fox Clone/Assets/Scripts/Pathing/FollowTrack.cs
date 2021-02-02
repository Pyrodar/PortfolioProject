using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script uses a rigidbodys velocity to move so the velocity can be added to the gameobject for targeting purposes. 
/// The actual cart following the path uses the "Cart" script
/// </summary>
public class FollowTrack : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Cart Cart;
    bool go = false;
    Transform cartTransform;
    Rigidbody rigid;


    public TrackEndEvent trackEnded;


    private void Awake()
    {
        cartTransform = Cart.transform;
        rigid = GetComponent<Rigidbody>();

        if (go)
        {
            Cart.Speed = speed;
            Cart.Distance = 1f;
        }
    }

    private void Start()
    {
        transform.position = cartTransform.position;
    }

    private void FixedUpdate()
    {
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
        transform.LookAt(cartTransform);

        //ensuring cart always has the same distance (of 4) to this object
        float distance = Vector3.Distance(transform.position, cartTransform.position);
        Cart.Speed = speed * ((distance * (-1)) + 8);
    }

    public void StartFollow()
    {
        go = true;
        gameObject.SetActive(true);

        if (rigid != null) rigid.drag = 0f;

        Cart.Speed = speed;
    }

    public void StopFollow()
    {
        go = false;
        rigid.drag = 1f;
        Cart.Speed = 0f;
    }

    internal int getCurrentWaypoint()
    {
        //TODO: find out how to access last waypoint
        return 0;
    }

    bool IsEndReached()
    {
        return Cart.Distance >= Cart.PathLength;
    }

    
    public void debugSpeed(float i)
    {
        speed = i;
        //Cart.Speed = i;
    }
}


[Serializable]
public class TrackEndEvent : UnityEvent { }
