using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurretMount : MonoBehaviour, IVehicle
{
    [SerializeField] bool upperRight;
    [SerializeField] bool lowerRight;
    [SerializeField] bool lowerLeft;
    [SerializeField] bool upperLeft;

    List<int> quarters = new List<int>();
    public List<int> Quarters
    {
        get { return quarters; }
    }

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
    
    [SerializeField]
    Turret myTurret;
    public Turret MyTurret
    {
        get { return myTurret; }
    }
    public TurretType MyTurretType
    {
        get { return myTurret.TurretType; }
    }
    
    [SerializeField]
    List<AquiredTarget> Targets = new List<AquiredTarget>();
    [SerializeField]
    List<AquiredTarget> Missles = new List<AquiredTarget>();


    public delegate void InformUI();
    public InformUI TurretDestroyed;

    #region GeneralSetup
    private void Awake()
    {
        if (upperRight) quarters.Add(0);
        if (lowerRight) quarters.Add(1);
        if (lowerLeft) quarters.Add(2);
        if (upperLeft) quarters.Add(3);
        if (GetComponentInChildren<Turret>() != null)
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

    public AquiredTarget getClosestMissle()
    {
        if (Missles.Count == 0) return null;

        AquiredTarget priorityTarget = Missles[0];
        float distance = Vector3.Distance(priorityTarget.transform.position, transform.position);


        for (int i = 1; i < Missles.Count; i++)
        {
            float distanceNew = Vector3.Distance(Missles[i].transform.position, transform.position);
            if (distance > distanceNew)
            {
                distance = distanceNew;
                priorityTarget = Missles[i];
            }
        }

        return priorityTarget;
    }

    #endregion

    #region MissleTurret
    public MissleTurret getMissleTurret()
    {
        if(myTurret.TurretType != TurretType.Missiles) return null;
        return myTurret.GetComponent<MissleTurret>();
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
