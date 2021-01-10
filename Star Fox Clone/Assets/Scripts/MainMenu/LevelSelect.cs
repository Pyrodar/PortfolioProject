using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] List<LevelData> levelsAvailable;
    [SerializeField] GridLayoutGroup levelGrid;
    [SerializeField] LevelSelectButton levelButtonPrefab;
    [SerializeField] Transform StartLevelPopUp;
    [SerializeField] Text StartLevelPopUpText;

    LevelData selectedLevel;

    private void Start()
    {
        foreach (LevelData ld in levelsAvailable)
        {
            LevelSelectButton lsb = Instantiate(levelButtonPrefab);
            lsb.Instantiate(ld, this);
            lsb.transform.SetParent(levelGrid.transform);
        }
    }

    public void OpenPopUp(LevelData ld)
    {
        selectedLevel = ld;
        StartLevelPopUpText.text = $"Do you want to play {ld.LevelName}?";
        StartLevelPopUp.gameObject.SetActive(true);
    }

    public void StartButton()
    {
        Debug.Log(selectedLevel.LevelName);
        GameStateConnection.Instance.LevelSelectedAndStartPressed(selectedLevel);
    }

    public void CancelButton() 
    {
        selectedLevel = null;
        StartLevelPopUp.gameObject.SetActive(false);
    }
}
