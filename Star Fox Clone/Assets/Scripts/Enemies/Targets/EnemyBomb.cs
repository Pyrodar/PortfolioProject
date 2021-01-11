using System;
using UnityEngine;
public class EnemyBomb : Target
{
    [SerializeField]BombData data;

    public void Initialize(BombData _data)
    {
        type = TargetType.plane;
        data = _data;
    }

    private void Update()
    {
        if (inDetectionRange()) followPlayer();

        if (minimumDistance()) detonate();
    }
    
    bool inDetectionRange()
    {
        if (data.speed <= 0) return false;

        return Vector3.Distance(transform.position, myTarget.transform.position) < data.detectionRange;
    }

    private void followPlayer()
    {
        if (data.speed <= 0) return;

        Vector3 interceptCourse = HelperFunctions.Intercept(transform.position, Vector3.zero, data.speed, myTarget.transform.position, myTarget.getVelocity());
        Vector3 direction = Vector3.Normalize(interceptCourse - transform.position);
        rigid.AddForce(direction * data.speed * Time.deltaTime);
    }

    /// <summary>
    /// used to determin when to detonate
    /// </summary>
    /// <returns> wether or not it is behind the Gameplay Plane </returns>
    private bool minimumDistance()
    {
        //Debug.Log("Z position bomb: " + myTarget.Plane.relativeZposition(transform.position));
        //Debug.Log("position bomb: " + transform.position);
        return myTarget.Plane.relativeZposition(transform.position) < 0; 
    }

    private void detonate()
    {
        Collider[] Colliders = HelperFunctions.SpawnExplosion(data.explosionVisuals, data.radius, transform.position);

        foreach (Collider hit in Colliders)
        {
            IVehicle t = hit.GetComponent<IVehicle>();
            if (t != null)
            {
                t.takeDamage(data.damage);
            }
        }

        destroySelf();
    }
}
