using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level Data", menuName = "Custom SO / Level Data")]
public class LevelData : ScriptableObject
{
    public Scene LevelToLoad;
    public string LevelName;
    public Sprite LevelPreviewImage;

    public int levelNumber;         //Will be used to set requirements to unlock levels

    #region HighScore

    [SerializeField] int numberOfTargets;
    int targetsDestroyed;

    float damageTaken;

    #endregion
}