using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLayoutInfo : MonoBehaviour
{
    #region singleton
    static MapLayoutInfo instance;

    public static MapLayoutInfo Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (MapLayoutInfo.instance != null)
        {
            Debug.LogWarning("More than one instance of MapLayoutInfo");
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    [SerializeField] Transform[] loadoutMapPlayerPositions;
    public Transform[] LoadoutMapPlayerPositions
    {
        get { return loadoutMapPlayerPositions; }
    }

    [SerializeField] GameplayPlane plane;
    public GameplayPlane Plane
    {
        get { return plane; }
    }

    [SerializeField] Camera[] cameras;
    public Camera[] Cameras
    {
        get { return cameras; }
    }
}
