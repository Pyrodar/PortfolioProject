using ProtocFiles;
using System.Collections.Generic;
using UnityEngine;

public class TurretMount : MonoBehaviour, IVehicle
{
    #region quarters
    [SerializeField] bool upperRight;
    [SerializeField] bool lowerRight;
    [SerializeField] bool lowerLeft;
    [SerializeField] bool upperLeft;

    List<int> quarters = new List<int>();
    public List<int> Quarters
    {
        get { return quarters; }
    }
    #endregion

    #region health and destruction
    [SerializeField] float TurretHP = 25;
    float currentHP;
    bool recharging = false;
    public bool Recharging
    {
        get { return recharging; }
    }
    
    [SerializeField] float respawnTime = 5;
    public float RespawnTime
    {
        get { return respawnTime; }
    }
    float timeOfRespawn;
    #endregion

    #region turret
    [SerializeField]
    Turret myTurret;
    public Turret MyTurret
    {
        get { return myTurret; }
    }
    public TurretClass_P MyTurretType
    {
        get { return myTurret.TurretType; }
    }

    Player playerReferenz;  //Referenz to get Player Velocity
    public Player PlayerReferenz { get { return playerReferenz; } set { playerReferenz = value; } }
    public Vector3 PlayerVelocity
    {
        get { return playerReferenz.Velocity; }
    }
    #endregion

    #region Targets
    List<AquiredTarget> Targets = new List<AquiredTarget>();
    List<AquiredTarget> Missles = new List<AquiredTarget>();
    #endregion

    #region UI
    public delegate void InformUI();
    public InformUI TurretDestroyed;
    #endregion

    #region GeneralSetup
    private void Awake()
    {
        if (upperRight) quarters.Add(0);
        if (lowerRight) quarters.Add(1);
        if (lowerLeft) quarters.Add(2);
        if (upperLeft) quarters.Add(3);
        //if (GetComponentInChildren<Turret>() != null)
        {
            myTurret = GetComponentInChildren<Turret>();
        }
        currentHP = TurretHP;
    }
    private void Update()
    {
        if (recharging && Time.time > timeOfRespawn)
        {
            reactivate();
        }  
    }

    public void SetTurret(Turret t)
    {
        myTurret = t;
    }

    public TurretData GetTurretData()
    {
        TurretData retVal = null;

        if(myTurret != null) retVal = myTurret.Data;

        return retVal;
    }
    #endregion

    #region Anti Ground
    public void AddTarget(AquiredTarget t)
    {
        if (t == null) return;
        if (Targets.Contains(t)) return;

        Targets.Add(t);
    }

    public void clearTargets()
    {
        Targets.Clear();
    }

    public AquiredTarget getPriorityTarget()
    {
        if (Targets.Count == 0) return null;
        return Targets[0];
    }
    #endregion

    #region AMS
    public void AddMissle(AquiredTarget t)
    {
        if (t == null) return;
        if (Missles.Contains(t)) return;

        Missles.Add(t);
    }

    public void clearMissles()
    {
        Missles.Clear();
    }

    /// <summary>
    /// gives a target to its AMS turret
    /// TODO: ignore targets that are not in range
    /// </summary>
    /// <returns></returns>
    public AquiredTarget getClosestMissle()
    {
        if (Missles.Count == 0) return null;
        //---->At least one Missle can be targeted

        AquiredTarget priorityTarget = Missles[0];
        float distance = 0;

        foreach (AquiredTarget missle in Missles)
        {
            float distanceNew = Vector3.Distance(missle.transform.position, transform.position);
            if (distance > distanceNew)
            {
                distance = distanceNew;
                priorityTarget = missle;
            }
        }
        return priorityTarget;
    }

    public AquiredTarget getClosestMissleFlak(float minRange)
    {
        if (Missles.Count == 0) return null;

        AquiredTarget priorityTarget = Missles[0];
        float distance = Vector3.Distance(priorityTarget.transform.position, transform.position);

        if (distance < minRange)
        {
            priorityTarget = null;
            distance = 1000;
        }

        for (int i = 1; i < Missles.Count; i++)
        {
            float distanceNew = Vector3.Distance(Missles[i].transform.position, transform.position);
            if (distanceNew < minRange) continue;

            if (distance > distanceNew)
            {
                distance = distanceNew;
                priorityTarget = Missles[i];
            }
        }

        return priorityTarget;
    }

    public void MisslesChanged()
    {
        AMSTurret tur = (AMSTurret)MyTurret;
        tur.MisslesChanged();
    }

    #endregion

    #region MissleTurret
    public MSLTurret getMissleTurret()
    {
        if(myTurret.TurretType != TurretClass_P.Msl) return null;
        return myTurret.GetComponent<MSLTurret>();
    }
    #endregion

    #region destruction

    public void takeDamage(float dmg)
    {
        if (currentHP <= 0) return;
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            destroySelf();
        }
    }

    public void takeDamage(float dmg, DamageType damageType)
    {
        switch (damageType)
        {
            default:
                takeDamage(dmg);
                break;
        }
    }

    public void destroySelf()
    {
        myTurret.gameObject.SetActive(false);
        timeOfRespawn = Time.time + RespawnTime;
        recharging = true;
        TurretDestroyed.Invoke();
    }

    void reactivate()
    {
        myTurret.gameObject.SetActive(true);
        currentHP = TurretHP;
        recharging = false;
    }
    #endregion
}
