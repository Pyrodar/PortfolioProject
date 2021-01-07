using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTrigger : MonoBehaviour
{
    [SerializeField] List<FollowTrack> connectedPlanes;
    [SerializeField] float relativeZStartingDistance;

    GameplayPlane plane;
    bool active = false;
    void Start()
    {
        plane = GameStateConnection.Instance.Plane;
    }

    void Update()
    {
        if (!active)
        {
            if (inPositionToStart()) activate();
        }
    }

    bool inPositionToStart()
    {
        return relativeZStartingDistance > plane.relativeZposition(transform.position) && plane.relativeZposition(transform.position) > (relativeZStartingDistance - 5);
    }

    void activate()
    {
        active = true;
        foreach (var plane in connectedPlanes)
        {
            plane.Go = true;
        }
    }
}
