using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class GameplayPlane : MonoBehaviour
{
    [SerializeField] int maxWidth = 10;
    [SerializeField] int maxHeight = 6;
    Rigidbody rigid;
    
    public int MaxWidth
    {
        get { return maxWidth; }
    }
    public int MaxHeight
    {
        get { return maxHeight; }
    }


    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public float relativeZposition(Vector3 pos)
    {
        Vector3 relativePos = transform.InverseTransformPoint(pos);
        return relativePos.z;
    }

    internal Vector3 getVelocity()
    {
        return rigid.velocity;
    }
}
