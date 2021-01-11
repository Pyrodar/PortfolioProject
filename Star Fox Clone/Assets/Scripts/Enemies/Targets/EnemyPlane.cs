using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FollowTrack))]
public class EnemyPlane : Target
{
    [SerializeField] int startShootingAfterWaypoint;
    int currentWaypoint;

    List<StationaryWeapon> stationaryWeapons;

    FollowTrack track;

    protected override void Start()
    {
        stationaryWeapons = new List<StationaryWeapon>();
        foreach (StationaryWeapon sw in GetComponentsInChildren<StationaryWeapon>())
        {
            stationaryWeapons.Add(sw);
        }

        track = GetComponent<FollowTrack>();
        base.Start();
    }

    void getCurrentWaypoint()
    {
        currentWaypoint = track.getCurrentWaypoint();
        Debug.Log("currentWaypoint: " + currentWaypoint);
    }

    public override void destroySelf()
    {
        track.Go = false;
        rigid.useGravity = true;

        foreach (StationaryWeapon sw in stationaryWeapons)
        {
            sw.destroySelf();
        }

        Invoke("crash", 2f);
    }

    void crash()
    {
        base.destroySelf();
    }
}
