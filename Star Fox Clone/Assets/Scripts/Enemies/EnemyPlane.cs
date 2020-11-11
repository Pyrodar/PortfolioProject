using UnityEngine;
[RequireComponent(typeof(FollowTrack))]
public class EnemyPlane : Target
{
    [SerializeField] int startShootingAfterWaypoint;
    int currentWaypoint;
    FollowTrack track;

    protected override void Start()
    {
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
        Invoke("crash", 2f);
    }

    void crash()
    {
        base.destroySelf();
    }
}
