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
    [SerializeField] protected float alternativeRange = -1;

    [SerializeField] protected Sprite impactMarkersprite;
    protected GameObject impactMarker;

    [Tooltip("If synchronized is active there will be no random element to the reload time")]
    [SerializeField] protected bool synchronized;

    protected Player myTarget;

    protected float timeWhenReloaded;

    protected bool isActive = true;

    protected virtual void Update()
    {
        if (!isActive) return;

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
        GameConnection.Instance.switchingPlayers += changeTarget;
        //Debug.Log("Added object: " + name + " to list of switch player delegate");

        changeTarget();
    }

    void changeTarget()
    {
        if (GameConnection.Instance == null)
        {
            myTarget = null;
            return;
        }
        myTarget = GameConnection.Instance.getFrontlinePlayer();
    }

    protected bool InSights(Vector3 interceptPoint)
    {
        return HelperFunctions.LinedUp(interceptPoint, transform.position, transform.forward, .8f);
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
    /// Aims directly for the player. Should be overwritten in each subtype
    /// Missles have their own funktion, since they are designed to shoot above the player and descend afterwards
    /// </summary>
    protected virtual void aim()
    {
        Vector3 interceptPoint = getInterceptPoint();
        if (interceptPoint.magnitude == 0) return;

        HelperFunctions.LookAt(transform, interceptPoint, data.turretSpeed, RotationParent.up);
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
        return Time.time >= timeWhenReloaded;
    }

    protected bool isInRange()
    {
        if (myTarget == null || myTarget.Plane == null)
        { 
            changeTarget(); 
            return false; 
        }

        float distance = Vector3.Distance(myTarget.transform.position, transform.position);

        //checks if target is too close
        if (myTarget.Plane.relativeZposition(transform.position) < 20) return false;

        //checks if alternative range will be used
        if (alternativeRange > 0 && distance <= alternativeRange) return true;

        //checks if in turret range
        if (distance <= data.turretRange) return true;

        //not in range
        return false;
    }

    protected virtual void placeMarker(Vector3 pos)
    {
        if (impactMarker == null)
        {
            impactMarker = new GameObject();
            SpriteRenderer s = impactMarker.AddComponent<SpriteRenderer>();
            s.sprite = impactMarkersprite;
        }

        #region taking own velocity into account
        Vector3 addedVelocity = Vector3.zero;

        if (Vector3.Magnitude(MyVelocity) > .5f)
        { 
            float distance = Vector3.Distance(myTarget.transform.position, transform.position);
            float time = distance / data.bulletData.speed;
            addedVelocity = MyVelocity * time;
        }
        #endregion

        impactMarker.transform.position = pos + addedVelocity;
        impactMarker.transform.rotation = myTarget.Plane.transform.rotation;
    }
    #endregion

    #region destruction
    public virtual void destroySelf()
    {
        GameConnection.Instance.switchingPlayers -= changeTarget;
        //Debug.Log("Removed object: " + name + " to list of switch player delegate");
        Destroy(this.gameObject);
    }
    #endregion

    public void SetParent(Target parent)
    {
        myHost = parent;
    }

    public void AllowFire(bool isAllowed)
    {
        isActive = isAllowed;
    }
}
