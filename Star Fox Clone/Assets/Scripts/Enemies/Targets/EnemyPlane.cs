using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FollowTrack))]
public class EnemyPlane : Target
{
    [SerializeField] int startShootingAfterWaypoint;
    int currentWaypoint
    {
        get { return track.getCurrentWaypoint(); }
    }

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

        if (startShootingAfterWaypoint > 0)
        {
            foreach (var sw in stationaryWeapons) sw.AllowFire(false);
        }
    }


    void Update()
    {
        if (currentWaypoint >= startShootingAfterWaypoint)
        {
            foreach(var sw in stationaryWeapons) sw.AllowFire(true);
            enabled = false;
        }

        //Debug.Log("currentWaypoint: " + currentWaypoint);
        //Debug.Log("Allowing Fire: " + (currentWaypoint >= startShootingAfterWaypoint).ToString());
    }

    public override void destroySelf()
    {
        track.StopFollow();

        rigid.AddRelativeForce(Vector3.forward * track.Speed * 1.6f, ForceMode.Impulse);
        rigid.useGravity = true;
        rigid.drag = 0f;

        float hurlRange = .7f;
        rigid.angularVelocity = new Vector3(Random.Range(-hurlRange, hurlRange), Random.Range(-hurlRange, hurlRange), Random.Range(-hurlRange, hurlRange));

        foreach (StationaryWeapon sw in stationaryWeapons)
        {
            sw.destroySelf();
        }

        //removes this object as target
        foreach (var player in Players)
        {
            player.removeMarkedTarget(this);
        }

        Invoke("crash", 2f);
    }

    void crash()
    {
        base.destroySelf();
    }
}
