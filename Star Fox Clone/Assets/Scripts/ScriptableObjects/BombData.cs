using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Data", menuName = "Custom SO / Bomb Data")]
public class BombData : ScriptableObject
{
    public float damage;
    public float radius;
    public float detectionRange;
    public float speed;

    public GameObject visuals;
    public GameObject explosionVisuals;
}

