using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PracticeTarget : MonoBehaviour
{
    public Vector3 vel;
    public Vector3 angVel;

    Rigidbody rigid;

    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        rigid.velocity = transform.TransformDirection(vel);
        rigid.angularVelocity = angVel;
    }

    private void Update()
    {
        rigid.velocity = transform.TransformDirection(vel);

    }

    public Vector3 getVelocity()
    {
        return rigid.velocity;
    }
}
