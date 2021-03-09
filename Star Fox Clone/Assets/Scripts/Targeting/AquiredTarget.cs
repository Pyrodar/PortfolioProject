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
    
    /// <summary>
    /// Only exists for Mirror
    /// </summary>
    public AquiredTarget()
    {
        Debug.LogWarning("Undefined Target created");
        transform = null;
        Velocity = Vector3.zero;
        CurrentQuarter = 0;
        Type = TargetType.missle;
    }

    public void UpdateVelocity()
    {
        Velocity = transform.GetComponent<Target>().Velocity;
        //AVelocity = transform.GetComponent<Target>().AVelocity;
    }
}

