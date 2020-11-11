using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraScript : MonoBehaviour
{
    public GameplayPlane plane;
    public Player player;
    public Transform bigCrossHair;
    public Transform debugTarget;
    public Transform debugMarker;

    public Vector2 offset = new Vector2(0, 3);

    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 1f;

    private void FixedUpdate()
    {
        followPlayer();
        ClampPosition();
    }

    void followPlayer()
    {
        Vector3 localPos = transform.localPosition;
        Vector3 playerPos = player.transform.localPosition;
        Vector3 targetPos = playerPos / 2;
        transform.localPosition = Vector3.SmoothDamp(localPos,new Vector3(playerPos.x + offset.x, playerPos.y + offset.y, localPos.z), ref velocity, smoothTime);
    }

    void ClampPosition()
    {
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.Clamp(localPos.x, -plane.MaxWidth, plane.MaxWidth), Mathf.Clamp(localPos.y, -plane.MaxHeight, plane.MaxHeight), localPos.z);
    }

    public int giveLocationRelativeToCrosshair(Transform target)
    {
        int retVal = 0;

        //sending raycast to target and aquiring position when at around same height as crosshair:
        float range = Vector3.Distance(transform.position, bigCrossHair.position);
        Camera cam = GetComponent<Camera>();
        Vector3 screenPoint = cam.WorldToScreenPoint(target.position);
        Ray ray = cam.ScreenPointToRay(screenPoint);
        Vector3 placeToCheck = transform.position + ray.direction * range;
        Vector3 placeConverted = bigCrossHair.InverseTransformPoint(placeToCheck);

        //Debugging:
        /*
        Debug.Log(placeConverted);
        Debug.DrawRay(cam.transform.position, ray.direction * 50, Color.red, 200);
        debugMarker.position = placeToCheck;
        */
        //checking wether target is below or above the crosshair:

        if (placeConverted.x <= 0)
        {
            retVal = 2;
            if (placeConverted.y > 0)
            {
                retVal += 1;
            }
        }
        else if (placeConverted.y < 0)
        {
            retVal += 1;
        }

        //weirdly stacked ifs but they result in a nice clockwise increase in value:
        //
        //  3   |   0 
        //------+------
        //  2   |   1
        //      
        
        return retVal;
    }
}

public enum CrosshairQuarter
{
    upperRight = 0
    , lowerRight = 1
    , lowerLeft = 2
    , upperLeft = 3
}
