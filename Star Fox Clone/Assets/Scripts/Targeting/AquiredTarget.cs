using UnityEngine;
[System.Serializable]
public class AquiredTarget
{
    public Transform transform;
    public Vector3 velocity;
    public int currentQuarter;
    public TargetType type;
    
    public AquiredTarget(Transform t, Vector3 _velocity, int quarter, TargetType _type)
    {
        transform = t;
        velocity = _velocity;
        currentQuarter = quarter;
        type = _type;
    }

}

public enum TargetType
{
    missle
    , vehicle
    , plane
}
