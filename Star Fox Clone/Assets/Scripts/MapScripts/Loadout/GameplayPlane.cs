using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class GameplayPlane : MonoBehaviour
{
    //Debugging############
    [SerializeField] GameConnection gamePrefab;
    //######################

    [SerializeField] int maxWidth = 10;
    [SerializeField] int maxHeight = 6;
    Rigidbody rigid;

    [SerializeField] Transform[] playerpositions;
    public Transform[] Playerpositions { get { return playerpositions; } }

                                                  //Front                //Back
    Vector3[] desiredPositions = new Vector3[2] { new Vector3(0, 0, 0) , new Vector3(0, 0, -15) };
    bool inPosition;
    float lerpValue;
    float switchTime = 5f; //how long to reach new position

    public int MaxWidth
    {
        get { return maxWidth; }
    }
    public int MaxHeight
    {
        get { return maxHeight; }
    }

    public Vector3 Velocity
    {
        get {
                FollowTrack followTrack = gameObject.GetComponent<FollowTrack>();
                if (followTrack)
                {
                    return followTrack.Velocity;
                }
                return Vector3.zero; 
            }
    }
    
    public FollowTrack Track
    {
        get {
                FollowTrack followTrack = gameObject.GetComponent<FollowTrack>();
                if (followTrack)
                {
                    return followTrack;
                }
                return null; 
            }
    }


    [SerializeField] GameplayBoundaries gameplayBoundaries;
    public GameplayBoundaries GameplayBoundaries
    {
        get { return gameplayBoundaries; }
    }

    private void Start()
    {
        //Debugging############
        if (GameConnection.Instance == null) loadGameStateCon();
        //######################

        rigid = GetComponent<Rigidbody>();
        inPosition = true;
    }

    //Debugging#####################
    void loadGameStateCon()
    {
        GameConnection game = Instantiate(gamePrefab);
        game.SetConnectionType(ConnectionType.SinglePlayer);
        game.StartGameMap();
    }
    //#############################
    public float relativeZposition(Vector3 pos)
    {
        Vector3 relativePos = transform.InverseTransformPoint(pos);
        return relativePos.z;
    }

    [Obsolete("Use GameplayPlane.Velocity instead")]
    internal Vector3 getVelocity()
    {
        return Velocity;
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
        lerpValue = 0;
    }

    /// <summary>
    /// For moving players
    /// And debugging
    /// </summary>
    private void Update()
    {
        #region Moving Players

        if (inPosition) return;

        for (int i = 0; i < playerpositions.Length; i++)
        {
            playerpositions[i].localPosition = Vector3.Lerp(desiredPositions[(i + 1) % 2], desiredPositions[i], lerpValue);
        }

        lerpValue += (Time.deltaTime / switchTime);
        if (lerpValue >= 1)
        {
            inPosition = true;
        }
        #endregion
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
