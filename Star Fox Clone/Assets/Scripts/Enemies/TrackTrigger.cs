using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTrigger : MonoBehaviour
{
    [SerializeField] List<FollowTrack> connectedPlanes;
    [SerializeField] float relativeZStartingDistance;

    Player player;
    bool active = false;
    void Start()
    {
        player = Player.Instance;
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
        return relativeZStartingDistance > player.Plane.relativeZposition(transform.position) && player.Plane.relativeZposition(transform.position) > (relativeZStartingDistance - 5);
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
