using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "Hostile Turret Data", menuName = "Custom SO / Hostile Turret Data")]
public class HostileTurretData : ScriptableObject
{
    public HostileTurretType turretType;

    public float turretRange;
    public float bulletspread;
    public float turretSpeed;
    public float cooldown;
    public int bulletsPerSalvo;

    public float ejectSpeed;

    public MissleData missleData;
    public BulletData bulletData;
    public BombData bombData;
}

public enum HostileTurretType
{
    Bombs
    , AntiAir
    , Missiles
}

[CustomEditor(typeof(HostileTurretData))]
[CanEditMultipleObjects]
public class HostileTurretDataEditor : Editor
{
    private void OnEnable()
    {
        EditorUtility.SetDirty(target);
    }
    public override void OnInspectorGUI()
    {
        var t = target as HostileTurretData;
        t.turretType = (HostileTurretType)EditorGUILayout.EnumPopup("Turret Type", t.turretType);
        if (t.turretType == HostileTurretType.Missiles)
        {
            t.turretRange = EditorGUILayout.FloatField("Turret Range", t.turretRange);
            t.missleData = EditorGUILayout.ObjectField("MissleData", t.missleData, typeof(MissleData), true) as MissleData;
            t.bulletsPerSalvo = EditorGUILayout.IntField("Missles per Salvo", t.bulletsPerSalvo);
            t.cooldown = EditorGUILayout.FloatField("Missle Reload Time", t.cooldown);
            t.ejectSpeed = EditorGUILayout.FloatField("Missle Eject Speed", t.ejectSpeed);
        }
        else if (t.turretType == HostileTurretType.AntiAir)
        {
            t.bulletData = EditorGUILayout.ObjectField("BulletData", t.bulletData, typeof(BulletData), true) as BulletData;
            t.bulletsPerSalvo = EditorGUILayout.IntField("Bullets per Salvo", t.bulletsPerSalvo);
            t.turretRange = EditorGUILayout.FloatField("Turret Range", t.turretRange);
            t.bulletspread = EditorGUILayout.Slider("Bullet Spread", t.bulletspread, 0f, 2f);
            t.turretSpeed = EditorGUILayout.FloatField("Turret Speed", t.turretSpeed);
            t.cooldown = EditorGUILayout.FloatField("Reload Time", t.cooldown);
            t.ejectSpeed = EditorGUILayout.Slider("RateOfFire", t.ejectSpeed, 0.01f, 0.5f);
        }
        else if(t.turretType == HostileTurretType.Bombs)
        {
            t.turretRange = EditorGUILayout.FloatField("Turret Range", t.turretRange);
            t.bombData = EditorGUILayout.ObjectField("BombData", t.bombData, typeof(BombData), true) as BombData;
            t.cooldown = EditorGUILayout.FloatField("Reload Time", t.cooldown);
        }
    }
}