using System.Collections;
using UnityEngine;

public class StationaryWeapon : MonoBehaviour
{
    [SerializeField] protected Transform RotationParent;

    [SerializeField] protected HostileTurretData data;

    [SerializeField] protected Sprite impactMarkersprite;

    protected GameObject impactMarker;

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
        myTarget = GameStateConnection.Instance.getFrontlinePlayer();
        if (RotationParent == null)
        {
            RotationParent = transform.parent;
        }
        GameStateConnection.Instance.switchingPlayers += changeTarget;
        //Debug.Log("Added object: " + name + " to list of switch player delegate");
    }

    void changeTarget()
    {
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
    /// Aims directly for the player. Missles have their own funktion, since they are designed to shoot above the player and descend afterwards
    /// </summary>
    protected virtual void aim()
    {
        Vector3 InterceptPoint = getInterceptPoint();
        if (InterceptPoint.magnitude == 0) return;

        HelperFunctions.LookAt(transform, InterceptPoint, data.turretSpeed, RotationParent.up);
    }

    /// <summary>
    /// better to override this funktion pretending on whether bullets or missles are used
    /// </summary>
    /// <returns>point to shoot in order to hit the target with it's current velocity</returns>
    protected virtual Vector3 getInterceptPoint()
    {
        return HelperFunctions.Intercept(transform.position, Vector3.zero, data.bulletData.speed, myTarget.transform.position, myTarget.getVelocity());
    }

    protected void startReloading()
    {
        if (Time.time > timeWhenReloaded)
        {
            timeWhenReloaded = Time.time + data.cooldown;
        }
    }

    protected bool isLoaded()
    {
        return Time.time > timeWhenReloaded;
    }

    protected bool isInRange()
    {
        if (myTarget == null) { changeTarget(); return false; }

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
}
