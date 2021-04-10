using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship Data", menuName = "Custom SO / ShipData")]
public class ShipData : ScriptableObject
{
    #region Movement
    public float moveSpeed = 4.6f;
    public float rotationSpeed = 0.7f;

    public float fokusPointRange = 2f;
    public float fokusPointSpeed = 0.04f;
    public float fokusPointDamping = 3f;

    [Tooltip("how fast the ship returns to looking straight forward again. fastest: 0.0 slowest: 1.0")]
    [Range(0.9f, 1f)]
    public float fokusPointCenteringSpeed = 0.995f;

    [Tooltip("how close to the ships front the crosshair returns after stopping the inputs")]
    public float fokusPointCenteringTolerance = 0.01f;

    public float gravityAndLift = 0.4f;

    public Vector3 crosshairOffset;
    #endregion

    #region Combat
    public float maxHealth = 100;
    public float invulnTime = 1;
    #endregion

    #region Turrets

    #endregion
}
