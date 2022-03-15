using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLayoutInfo : MonoBehaviour
{
    #region singleton
    static MapLayoutInfo instance;
    [SerializeField] InGameMenu menu;

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

        //menu = gameObject.GetComponentInChildren<InGameMenu>();
    }
    #endregion

    #region LoadoutMap
    [SerializeField] Transform[] loadoutMapPlayerPositions;
    public Transform[] LoadoutMapPlayerPositions
    {
        get { return loadoutMapPlayerPositions; }
    }


    public void StartGame()
    {
        GameConnection.Instance.LoadGameMap();
    }
    #endregion

    #region GameMap
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


    [SerializeField] UIBaseClass[] hud;
    public UIBaseClass[] HUD
    {
        get { return hud; }
    }

    [SerializeField] BulletFactory bulletFactory;
    public BulletFactory BulletFactory
    {
        get { return bulletFactory; }
    }

    #region Score

    int targetsDestroyed;
    float damageTaken;

    #endregion

    #endregion

    #region IngameMenu
    public void OpenMenu()
    {
        Debug.Log("Toggeling Menu");
        menu.ToggleMenu();
    }
    #endregion
}
