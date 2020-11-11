﻿using UnityEngine;
[CreateAssetMenu(fileName = "Missle Data", menuName = "Custom SO / Missle Data")]
public class MissleData : ScriptableObject
{
    public float damage;
    public float speed;
    public float turnSpeed;
    [Range(0.2f, 3f)]
    public float drag;
    [Tooltip("how long until the missle starts following the Target")]
    public float timeBeforeArmed;
    public float detectionRange;
    [Range(0.2f, 6)]
    public float missleSize;
    public GameObject visuals;
    public GameObject explosionVisuals;
}