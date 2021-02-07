using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent OnTrigger;
    [SerializeField] float relativeZStartingDistance;

    GameplayPlane plane;
    bool active = false;

    void Start()
    {
        plane = GameStateConnection.Instance.Plane;
    }

    void Update()
    {
        if (plane == null) { Start(); return; }

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
        OnTrigger?.Invoke();
    }
}
