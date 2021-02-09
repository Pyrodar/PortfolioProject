using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
/// <summary>
/// Might be unnessesary since only the carts position is important and that can be calculated in the Pathfollow script
/// </summary>
public class Cart : MonoBehaviour
{
    [SerializeField]PathCreator Path;

    public float Speed = 0;
    public float Distance = 0;
    public float PathLength
    {
        get { return Path.path.length; }
    }

    private void Awake()
    {
        transform.position = Path.path.GetPointAtDistance(0);
    }

    private void Update()
    {
        Distance += Speed * Time.deltaTime;
        transform.position = Path.path.GetPointAtDistance(Distance);
        //transform.rotation = Path.path.GetRotationAtDistance(distanceTraveled);
    }
}