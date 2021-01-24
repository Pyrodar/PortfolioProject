using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "Bullet Data", menuName = "Custom SO / Bullet Data")]
public class BulletData : ScriptableObject
{
    public float damage;
    public float speed;
    public float lifetime;
    public GameObject visuals;
    public DamageType damageType;
    public float radius;
    public GameObject explosionVisuals;
}

[CustomEditor(typeof(BulletData))]
[CanEditMultipleObjects]
public class BulletDataEditor : Editor
{
    private void OnEnable()
    {
        EditorUtility.SetDirty(target);
    }
    public override void OnInspectorGUI()
    {
        var t = target as BulletData;

        t.damage = EditorGUILayout.FloatField("Damage", t.damage);
        t.speed = EditorGUILayout.FloatField("Bullet Speed", t.speed);
        t.lifetime = EditorGUILayout.FloatField("Lifetime", t.lifetime);
        t.visuals = EditorGUILayout.ObjectField("Visuals", t.visuals, typeof(GameObject), true) as GameObject;

        t.damageType = (DamageType)EditorGUILayout.EnumPopup("Damage Type", t.damageType);

        switch (t.damageType)
        {
            case DamageType.highExplosive:
                t.radius = EditorGUILayout.FloatField("Explosion Radius", t.radius);
                t.explosionVisuals = EditorGUILayout.ObjectField("Explosion Visuals", t.explosionVisuals, typeof(GameObject), true) as GameObject;
                break;
            case DamageType.flak:
                t.radius = EditorGUILayout.FloatField("Explosion Radius", t.radius);
                t.explosionVisuals = EditorGUILayout.ObjectField("Explosion Visuals", t.explosionVisuals, typeof(GameObject), true) as GameObject;
                break;
            default:
                break;
        }
    }
}
