using ProtocFiles;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "Turret Data", menuName = "Custom SO / Turret Data")]
public class TurretData : ScriptableObject
{
    public TurretClass_P turretType;
    public GameObject TurretMesh;

    public float turretRange;
    public float bulletSpread;
    public float turretSpeed;
    public float cooldown;

    public float heatBuildup;
    public float heatDissipation;

    public bool firingDiscipline;

    public int missleSpace;
    public float ejectSpeed;

    public MissleData missleData;
    public BulletData bulletData;

    public Sprite Icon;
    public string Description = "No description currently available";

    public SmallTurretData GetSmallData()
    {
        SmallTurretData data = new SmallTurretData() { 
        turretType = this.turretType,
        turretRange = this.turretRange,
        bulletSpread = this.bulletSpread,
        ejectSpeed = this.ejectSpeed,
        missleData = this.missleData,
        bulletData = this.bulletData };

        return data;
    }
}
/// <summary>
/// waqs used for mirror, since it can't process Sprites and doesn't need all information anyway
/// </summary>
public class SmallTurretData
{
    public TurretClass_P turretType;
    public float turretRange;
    public float bulletSpread;
    public float ejectSpeed;
    public MissleData missleData;
    public BulletData bulletData;
}

//public enum TurretType -> moved to using the enum TurretClass_P from the Protobuf files
//{
//    AMS
//    , ATG
//    , MSL
//}

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
        t.turretType = (TurretClass_P)EditorGUILayout.EnumPopup("Turret Type", t.turretType);
        t.TurretMesh = EditorGUILayout.ObjectField("Mesh", t.TurretMesh, typeof(GameObject), true) as GameObject;
        if (t.turretType == TurretClass_P.Msl)
        {
            t.missleData = EditorGUILayout.ObjectField("MissleData", t.missleData, typeof(MissleData), true) as MissleData;
            t.missleSpace = EditorGUILayout.IntField("Missle Space", t.missleSpace);
            t.cooldown = EditorGUILayout.FloatField("Missle Reload Time", t.cooldown);
            t.ejectSpeed = EditorGUILayout.FloatField("Missle Eject Speed", t.ejectSpeed);
        }
        else if(t.turretType == TurretClass_P.Atg)
        {
            t.bulletData = EditorGUILayout.ObjectField("BulletData", t.bulletData, typeof(BulletData), true) as BulletData;
            t.bulletSpread = EditorGUILayout.Slider("Bullet Spread", t.bulletSpread, 0f, 1f);
            t.turretSpeed = EditorGUILayout.Slider("Turret Speed", t.turretSpeed, 1f, 36f);
            t.cooldown = EditorGUILayout.Slider("Reload Time", t.cooldown, 0f, 3f);
            t.firingDiscipline = EditorGUILayout.Toggle("Firing Discipline", t.firingDiscipline);
            if (t.firingDiscipline)
            {
                t.turretRange = EditorGUILayout.FloatField("Turret Range", t.turretRange);
            }
        }
        else //TurretType.AMS
        {
            t.bulletData = EditorGUILayout.ObjectField("BulletData", t.bulletData, typeof(BulletData), true) as BulletData;
            t.turretRange = EditorGUILayout.FloatField("Turret Range", t.turretRange);
            t.bulletSpread = EditorGUILayout.Slider("Bullet Spread", t.bulletSpread, 0f, 1f);
            t.turretSpeed = EditorGUILayout.Slider("Turret Speed", t.turretSpeed, 1f, 36f);
            t.cooldown = EditorGUILayout.Slider("Reload Time", t.cooldown, 0f, 3f);
            t.heatBuildup = EditorGUILayout.Slider("Heat Buildup", t.heatBuildup, 0.01f, 5f);
            t.heatDissipation = EditorGUILayout.Slider("Heat Dissipation", t.heatDissipation, 1f, 10f);
        }
        t.Description = EditorGUILayout.TextField("Description:", t.Description);
    } 
}