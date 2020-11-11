using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayerConnector : MonoBehaviour
{
    [SerializeField] GameplayPlane plane;
    [SerializeField] Camera cam;

    private void Start()
    {
        Player.Instance.transform.parent = plane.transform;
        Player.Instance.StartGame(plane, cam);
        plane.GetComponent<FollowTrack>().Go = true;

        cam.GetComponent<CameraScript>().player = Player.Instance;
    }
}
