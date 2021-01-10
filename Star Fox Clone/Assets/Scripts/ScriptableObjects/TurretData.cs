using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "Turret Data", menuName = "Custom SO / Turret Data")]
public class TurretData : ScriptableObject
{
    public TurretType turretType;
    public GameObject emptyTurretMesh;

    public float turretRange;
    public float bulletspread;
    public float turretSpeed;
    public float cooldown;

    public bool firingDiscipline;

    public int missleSpace;
    public float ejectSpeed;

    public MissleData missleData;
    public BulletData bulletData;
}

public enum TurretType
{
    AMS
    , AntiGround
    , Missiles
}

[CustomEditor(typeof(TurretData))]
[CanEditMultipleObjects]
public class TurretDataEditor : Editor
{
    private void OnEnable()
    {
        EditorUtility.SetDirty(target);
    }
    public override void OnInspectorGUI()
    {
        var t = target as TurretData;
        t.turretType = (TurretType)EditorGUILayout.EnumPopup("Turret Type", t.turretType);
        if (t.turretType == TurretType.Missiles)
        {
            t.missleData = EditorGUILayout.ObjectField("MissleData", t.missleData, typeof(MissleData), true) as MissleData;
            t.missleSpace = EditorGUILayout.IntField("Missle Space", t.missleSpace);
            t.cooldown = EditorGUILayout.FloatField("Missle Reload Time", t.cooldown);
            t.ejectSpeed = EditorGUILayout.FloatField("Missle Eject Speed", t.ejectSpeed);
        }
        else if(t.turretType == TurretType.AntiGround)
        {
            t.bulletData = EditorGUILayout.ObjectField("BulletData", t.bulletData, typeof(BulletData), true) as BulletData;
            t.bulletspread = EditorGUILayout.FloatField("Bullet Spread", t.bulletspread);
            t.turretSpeed = EditorGUILayout.FloatField("Turret Speed", t.turretSpeed);
            t.cooldown = EditorGUILayout.FloatField("Reload Time", t.cooldown);
            t.firingDiscipline = EditorGUILayout.Toggle("Firing Discipline", t.firingDiscipline);
            if (t.firingDiscipline)
            {
                t.turretRange = EditorGUILayout.FloatField("Turret Range", t.turretRange);
            }
        }
        else
        {
            t.bulletData = EditorGUILayout.ObjectField("BulletData", t.bulletData, typeof(BulletData), true) as BulletData;
            t.turretRange = EditorGUILayout.FloatField("Turret Range", t.turretRange);
            t.bulletspread = EditorGUILayout.FloatField("Bullet Spread", t.bulletspread);
            t.turretSpeed = EditorGUILayout.FloatField("Turret Speed", t.turretSpeed);
            t.cooldown = EditorGUILayout.FloatField("Reload Time", t.cooldown);
        }
    } 
}