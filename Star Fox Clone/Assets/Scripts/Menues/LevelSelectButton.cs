using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] LevelData levelData;
    public LevelData LevelData { get { return levelData; } }
    [SerializeField] Image buttonImage;
    [SerializeField] Text buttonText;
    LevelSelect levelSelector;

    public void Instantiate(LevelData ld, LevelSelect ls)
    {
        levelData = ld;
        buttonImage.sprite = ld.LevelPreviewImage;
        buttonText.text = ld.LevelName;

        levelSelector = ls;
    }

    public void OnClicked()
    {
        levelSelector.OpenPopUp(levelData);
    }
}
