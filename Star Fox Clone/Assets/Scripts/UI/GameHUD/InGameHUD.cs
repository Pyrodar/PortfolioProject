﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameHUD : UIBaseClass
{
    private void Awake() { uiType = UIType.InGame; }

    #region HUD
    [SerializeField] VerticalBar[] healthbars;
    public VerticalBar[] Healthbars
    {
        get { return healthbars; }
    }


    [SerializeField] TurretIconList turretIconList;
    public TurretIconList TurretIconList
    {
        get { return turretIconList; }
    }

    [SerializeField] RectTransform[] targetMarkers;
    public RectTransform[] TargetMarkers
    {
        get { return targetMarkers; }
    }
    
    [SerializeField] SelectedMissleIcon missleIcon;
    public SelectedMissleIcon MissleIcon
    {
        get { return missleIcon; }
    }

    [SerializeField] GyroHorizon gyroHorizon;
    public GyroHorizon GyroHorizon
    {
        get { return gyroHorizon; }
    }

    #endregion

    #region GameOver

    [SerializeField] Transform GameOverScreen;

    public void GameOver()
    {
        GameOverScreen.gameObject.SetActive(true);
    }

    public void ReturnToMenuButton()
    {
        GameConnection.Instance.ReturnToMenu();
    }

    public void RestartButton()
    {
        GameConnection.Instance.RestartMap();
    }

    #endregion

    #region Victory

    [SerializeField] Transform VictoryScreen;
    public void Victory()
    {
        VictoryScreen.gameObject.SetActive(true);
    }

    #endregion
}
