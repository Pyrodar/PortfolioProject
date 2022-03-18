using System;
using UnityEngine;
using UnityEngine.Events;
using PathCreation;
using System.Collections.Generic;

/// <summary>
/// This script used a rigidbodys velocity to move so the velocity could be added to the gameobject for targeting purposes.
/// now it calculates its velocity based on its last position, since that allows more control over the current position and rotation
/// </summary>
public class FollowTrack : MonoBehaviour
{
    [SerializeField] PathCreator path;
    public PathCreator Path
    {
        get { return path; }
    }


    [SerializeField] float speed;
    public float Speed
    {
        get { return speed; }
    }
    bool go = false;

    #region specificPoints
    float pathPosition = 0;
    Vector3 PathPosition
    {
        get {
            if (!go) return Vector3.zero;
            
            return Path.path.GetPointAtDistance(pathPosition); 
        }
    }
    Vector3 CameraViewpointPosition
    {
        get {
            if (!go) return Vector3.zero;
            
            return Path.path.GetPointAtDistance(pathPosition + 5); 
        }
    }
    Vector3 PathNormal
    {
        get
        {
            if (!go) return Vector3.zero;

            return Path.path.GetNormalAtDistance(pathPosition);
        }
    }
    List<float> triggerPoints = new List<float>();
    #endregion

    #region Velocity
    Vector3 lastPosition = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    public Vector3 Velocity
    {
        get
        {   return velocity; }
    }
    #endregion


    public UnityEvent OnTrackEnded;
    public UnityEvent<float> OnTrackTriggerPointPassed;

    private void Update()
    {
        //DEBUGGING##################
        if (Input.GetKey(KeyCode.G))
        {
            debugSpeed(24);
        }
        else
        {
            debugSpeed(8);
        }

        //#########################
        
        if (!go) return;

        moveAlongPath();

        calculateVelocity();

        checkForTriggerPoints(pathPosition);
    }

    #region movement
    void calculateVelocity()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;

        lastPosition = transform.position;
    }

    void moveAlongPath()
    {
        pathPosition += speed * Time.deltaTime;

        transform.position = PathPosition;

        //Debug.Log("Normal : " + PathNormal);

        HelperFunctions.LookAt(transform, CameraViewpointPosition, 1f);

    }
    #endregion

    #region StartAndStop
    public void StartFollow()
    {
        pathPosition = 8;
        go = true;
        gameObject.SetActive(true);
        transform.position = PathPosition;

        //if (rigid != null) rigid.drag = 0f;
    }

    public void StopFollow()
    {
        go = false;
    }
    #endregion

    #region Way- and Triggerpoints
    internal int getCurrentWaypoint()
    {
        //TODO: find out how to access last waypoint
        float percentageDistance = (pathPosition / path.path.length) * 100;
        return (int) percentageDistance;
    }

    void checkForTriggerPoints(float pathPos)
    {
        #region End
        if (pathPosition >= path.path.length - 6)
        {
            StopFollow();
            OnTrackEnded.Invoke();
        }
        #endregion

        #region Triggers
        for (int i = 0; i < triggerPoints.Count; i++)
        {
            if (triggerPoints[i] <= pathPos)
            {
                OnTrackTriggerPointPassed.Invoke(triggerPoints[i]);
                triggerPoints.RemoveAt(i);
                break;
            }
        }
        #endregion
    }

    public void AddTriggerPoint(Vector3 TriggerPosition, out float pathPosition)
    {
        pathPosition = Path.path.GetClosestDistanceAlongPath(TriggerPosition);
        triggerPoints.Add(pathPosition);
    }

    #endregion


    public void debugSpeed(float i)
    {
        speed = i;
    }
}
