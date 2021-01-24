using UnityEngine;
[System.Serializable]
public class AquiredTarget
{
    public Transform transform;
    public Vector3 Velocity;
    public Vector3 AVelocity;
    public int CurrentQuarter;
    public TargetType Type;
    
    public AquiredTarget(Transform t, Vector3 velocity, int quarter, TargetType type)
    {
        transform = t;
        Velocity = velocity;
        CurrentQuarter = quarter;
        Type = type;
    }

    public void UpdateVelocity()
    {
        Velocity = transform.GetComponent<Target>().getVelocity();
        //AVelocity = transform.GetComponent<Target>().getAVelocity();
    }
}

