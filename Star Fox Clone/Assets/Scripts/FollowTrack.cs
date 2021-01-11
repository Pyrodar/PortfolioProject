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
    public bool Go = false;
    Transform cartTransform;
    Rigidbody rigid;


    public TrackEndEvent trackEnded;


    private void Awake()
    {
        cartTransform = cart.transform;
        cart.m_Speed = speed;
        cart.m_Position = 1f;
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!Go) return;

        moveAlongPath();

        if (IsEndReached())
        {
            Go = false;
            trackEnded.Invoke();
        }
    }

    public void StartFollow()
    {
        Go = true;
        gameObject.SetActive(true);
    }

    void moveAlongPath()
    {
        rigid.velocity = transform.forward * speed;
        transform.LookAt(cartTransform);

        //ensuring cart always has the same distance (of 1) to this object
        float distance = Vector3.Distance(transform.position, cartTransform.position);
        cart.m_Speed = speed * ((distance * (-1)) + 2);
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
}


[Serializable]
public class TrackEndEvent : UnityEvent { }
