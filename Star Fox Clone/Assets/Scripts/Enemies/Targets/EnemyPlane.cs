using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FollowTrack))]
public class EnemyPlane : Target
{
    [SerializeField] int startShootingAfterWaypoint;
    int currentWaypoint;

    List<StationaryWeapon> stationaryWeapons;

    FollowTrack track;

    public Vector3 PlaneVelocity
    {
        get
        {
            if (track)
            {
                return rigid.velocity + track.Velocity;
            }
            return rigid.velocity;
        }
    }

    protected override void Start()
    {
        stationaryWeapons = new List<StationaryWeapon>();
        foreach (StationaryWeapon sw in GetComponentsInChildren<StationaryWeapon>())
        {
            stationaryWeapons.Add(sw);
        }

        track = GetComponent<FollowTrack>();
        base.Start();
        type = TargetType.plane;
        gameObject.SetActive(false);    //hiding plane until it starts moving
    }

    void getCurrentWaypoint()
    {
        currentWaypoint = track.getCurrentWaypoint();
        Debug.Log("currentWaypoint: " + currentWaypoint);
    }

    public override void destroySelf()
    {
        track.StopFollow();
        rigid.useGravity = true;
        rigid.drag = 0f;

        float hurlRange = 1f;

        rigid.angularVelocity = new Vector3(Random.Range(-hurlRange, hurlRange), Random.Range(-hurlRange, hurlRange), Random.Range(-hurlRange, hurlRange));

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
