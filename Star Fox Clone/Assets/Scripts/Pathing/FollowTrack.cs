using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FollowTrack : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] CinemachineDollyCart cart;
    bool go = false;
    Transform cartTransform;
    Rigidbody rigid;


    public TrackEndEvent trackEnded;


    private void Awake()
    {
        cartTransform = cart.transform;
        rigid = GetComponent<Rigidbody>();

        if (go)
        {
            cart.m_Speed = speed;
            cart.m_Position = 1f;
        }
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

    public void StartFollow()
    {
        go = true;
        gameObject.SetActive(true);

        if (rigid != null) rigid.drag = 0f;

        cart.m_Speed = speed;
    }

    public void StopFollow()
    {
        go = false;
        rigid.drag = 1f;
        cart.m_Speed = 0f;
    }

    void moveAlongPath()
    {
        rigid.velocity = transform.forward * speed;
        transform.LookAt(cartTransform);

        //ensuring cart always has the same distance (of 4) to this object
        float distance = Vector3.Distance(transform.position, cartTransform.position);
        cart.m_Speed = speed * ((distance * (-1)) + 8);
    }

    internal int getCurrentWaypoint()
    {
        //TODO: find out how to access last waypoint
        return 0;
    }

    bool IsEndReached()
    {
        return cart.m_Position == cart.m_Path.PathLength;
    }

    public void debugSpeed(float i)
    {
        speed = i;
        cart.m_Speed = i;
    }
}


[Serializable]
public class TrackEndEvent : UnityEvent { }
