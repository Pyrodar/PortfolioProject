using Mirror;
using System.Collections;
using UnityEngine;

public class StationaryWeapon : MonoBehaviour
{
    [SerializeField] protected Transform RotationParent;
    protected Target myHost;
    protected Vector3 MyVelocity
    {
        get {
            if (myHost != null) return myHost.Velocity;
            else return Vector3.zero;    
        }
    }

    [SerializeField] protected HostileTurretData data;

    [SerializeField] protected Sprite impactMarkersprite;
    protected GameObject impactMarker;

    [Tooltip("If synchronized is active there will be no random element to the reload time")]
    [SerializeField] protected bool synchronized;

    protected Player myTarget;

    protected float timeWhenReloaded;

    private void Update()
    {
        if (isInRange())
        {
            aim();

            if (isLoaded()) StartCoroutine(Fire());
        }
    }

    #region targeting
    private void Start()
    {
        if (RotationParent == null)
        {
            RotationParent = transform.parent;
        }
        GameStateConnection.Instance.switchingPlayers += changeTarget;
        //Debug.Log("Added object: " + name + " to list of switch player delegate");

        changeTarget();
    }

    void changeTarget()
    {
        if (GameStateConnection.Instance == null)
        {
            myTarget = null;
            return;
        }
        myTarget = GameStateConnection.Instance.getFrontlinePlayer();
    }
    #endregion

    #region attacking
    /// <summary>
    /// has to be overwriten for each subtype
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Fire()
    {
        yield return new WaitForSeconds(1);
    }

    /// <summary>
    /// used as the network command to spawn each projectile
    /// the spawn information is passed to the Target since it is the only NetworkIdentity in this prefab
    /// </summary>
    protected virtual void spawnProjectile(GameObject projectile)
    {
        myHost.CmdSpawn(projectile);
        Destroy(projectile);
    }
    /// <summary>
    /// Aims directly for the player. Should be overwritten in each subtype
    /// Missles have their own funktion, since they are designed to shoot above the player and descend afterwards
    /// </summary>
    protected virtual void aim()
    {
        Vector3 InterceptPoint = getInterceptPoint();
        if (InterceptPoint.magnitude == 0) return;

        HelperFunctions.LookAt(transform, InterceptPoint, data.turretSpeed, RotationParent.up);
    }

    /// <summary>
    /// uses Bulletspeed, not missleSpeed
    /// better to override this funktion depending on whether bullets or missles are used
    /// </summary>
    /// <returns>point to shoot in order to hit the target with it's current velocity</returns>
    protected virtual Vector3 getInterceptPoint()
    {
        return HelperFunctions.Intercept(transform.position, MyVelocity, data.bulletData.speed, myTarget.transform.position, myTarget.Velocity);
    }

    protected void startReloading()
    {
        if (Time.time > timeWhenReloaded)
        {
            if (synchronized) timeWhenReloaded = Time.time + data.cooldown;
            else timeWhenReloaded = Time.time + data.cooldown + Random.Range(0, 1.5f); //Added some randomness so the same weapons don't all fire synchronized
        }
    }

    protected void skipLoading()
    {
        timeWhenReloaded = 0;
    }

    protected bool isLoaded()
    {
        return Time.time > timeWhenReloaded;
    }

    protected bool isInRange()
    {
        if (myTarget == null || myTarget.Plane == null)
        { 
            changeTarget(); 
            return false; 
        }

        float distance = Vector3.Distance(myTarget.transform.position, transform.position);
        if (distance > data.turretRange || myTarget.Plane.relativeZposition(transform.position) < 20) return false;

        return true;
    }

    protected virtual void placeMarker(Vector3 pos)
    {
        if (impactMarker == null)
        {
            impactMarker = new GameObject();
            SpriteRenderer s = impactMarker.AddComponent<SpriteRenderer>();
            s.sprite = impactMarkersprite;
        }

        impactMarker.transform.position = pos;
    }
    #endregion

    #region destruction
    public virtual void destroySelf()
    {
        GameStateConnection.Instance.switchingPlayers -= changeTarget;
        //Debug.Log("Removed object: " + name + " to list of switch player delegate");
        Destroy(this.gameObject);
    }
    #endregion

    public void SetParent(Target parent)
    {
        myHost = parent;
    }
}
