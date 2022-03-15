using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent OnTrigger;
    float PathTriggerPoint;

    GameplayPlane plane;
    bool active = false;

    void Start()
    {
        plane = GameConnection.Instance.Plane;
        if (plane)
        {
            plane.Track.AddTriggerPoint(transform.position, out PathTriggerPoint);
            plane.Track.OnTrackTriggerPointPassed.AddListener(OnTriggerPoint);
        }
        else
        {
            Debug.LogWarning("Not able to set Triggerpoint on path");
            Invoke("Start", 1f);
        }
    }

    void OnTriggerPoint(float pathPosition)
    {
        if (!active)
        {
            if (PathTriggerPoint <= pathPosition) activate();
        }
    }

    void activate()
    {
        active = true;
        OnTrigger?.Invoke();
    }
}
