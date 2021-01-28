﻿using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class GameplayPlane : MonoBehaviour
{
    [SerializeField] int maxWidth = 10;
    [SerializeField] int maxHeight = 6;
    Rigidbody rigid;

    [SerializeField] Transform[] playerpositions;
    public Transform[] Playerpositions { get { return playerpositions; } }

                                                  //Front                //Back
    Vector3[] desiredPositions = new Vector3[2] { new Vector3(0, 0, 0) , new Vector3(0, 0, -15) };
    bool inPosition;
    float lerpPosition;
    float switchTime = 5f; //how long to reach new position

    public int MaxWidth
    {
        get { return maxWidth; }
    }
    public int MaxHeight
    {
        get { return maxHeight; }
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        //playerpositions[0].localPosition = desiredPositions[0];
        //playerpositions[1].localPosition = desiredPositions[1];
        inPosition = true;
    }

    public float relativeZposition(Vector3 pos)
    {
        Vector3 relativePos = transform.InverseTransformPoint(pos);
        return relativePos.z;
    }

    internal Vector3 getVelocity()
    {
        return rigid.velocity;
    }

    public bool requestPlayerSwitch(int playerNumber)
    {
        if (!inPosition) return false;

        Debug.Log("Switch Player positions to " + playerNumber + " in front");
        forcePlayerSwitch();

        return true;
    }

    public void forcePlayerSwitch()
    {
        #region switching desired positions
        Vector3 vector = desiredPositions[0];
        desiredPositions[0] = desiredPositions[1];
        desiredPositions[1] = vector;
        #endregion

        inPosition = false;
        lerpPosition = 0;
    }

    /// <summary>
    /// Only for moving players
    /// </summary>
    private void Update()
    {
        if (inPosition) return;

        for (int i = 0; i < playerpositions.Length; i++)
        {
            playerpositions[i].localPosition = Vector3.Lerp(desiredPositions[(i + 1) % 2], desiredPositions[i], lerpPosition);
        }

        lerpPosition += (Time.deltaTime / switchTime);
        if (lerpPosition >= 1)
        {
            inPosition = true;
        }
    }

    public void PathEnded()
    {
        Debug.Log("Track Has Reached Its End");
        stopTrack();
        foreach (InGameHUD UI in MapLayoutInfo.Instance.HUD)
        {
            if (UI == null) continue;
            UI.Victory();
        }
    }

    public void stopTrack()
    {
        Debug.Log("STOPPED TRACK");
        GetComponent<FollowTrack>().StopFollow();
    }
}
