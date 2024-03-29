﻿using UnityEngine;

public class MSLTurret : Turret
{
    [Tooltip("how many Missles can be loaded at once")]
    private int maxMissles;
    private int currentMissles;
    public int CurrentMissles
    {
        get { return currentMissles; }
        set {
            currentMissles = value;
            if (myHudIcon != null) myHudIcon.SetMissles(currentMissles);
        }
    }

    private void Start()
    {
        maxMissles = Mathf.FloorToInt(data.missleSpace / data.missleData.missleSize);
        CurrentMissles = maxMissles;
    }

    private void Update()
    {
        loadMissles();
    }

    public override void ConnectToUI(TurretIcon icon)
    {
        base.ConnectToUI(icon);
        icon.SetMissles(currentMissles);
    }

    #region firing missle
    public void Fire(AquiredTarget target)
    {
        if (myMount.Recharging) return;

        //TODO: rotate through location of rockettubes and
        Transform tube = transform.GetChild(0);
        //############################################

        //Networking////////////////////
        var smallData = data.GetSmallData();
        myPlayer.CmdSpawnMissle(smallData, tube.position, tube.rotation, target);
        ////////////////////////////////

        startReloading();
    }
    #endregion

    #region loading missles
    void loadMissles()
    {
        if (CurrentMissles < maxMissles && Time.time > cooldownEnd)
        {
            CurrentMissles += 1;
            if (CurrentMissles < maxMissles)
            {
                cooldownEnd = Time.time + data.cooldown;
                if(myHudIcon != null) myHudIcon.LoadingMissle();
            }
            //Debug.Log("Loaded new Missle");
        }
    }

    void startReloading()
    {
        CurrentMissles -= 1;
        if (Time.time > cooldownEnd)
        {
            cooldownEnd = Time.time + data.cooldown;
            myHudIcon.LoadingMissle();
        }
    }

    public bool isLoaded()
    {
        if (CurrentMissles < 1) return false;
        return true;
    }
    #endregion
}