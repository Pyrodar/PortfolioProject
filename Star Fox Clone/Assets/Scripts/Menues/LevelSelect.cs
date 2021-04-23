using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] List<LevelData> levelsAvailable;
    [SerializeField] LevelMap map;


    [SerializeField] LevelSelectButton levelButtonPrefab;


    [SerializeField] Transform StartLevelPopUp;
    [SerializeField] Text StartLevelPopUpText;

    LevelData selectedLevel;
    List<LevelSelectButton> unlockedButtons = new List<LevelSelectButton>();

    private void Start()
    {
        DislplayLevels();
    }

    public void RefreshLevels()
    {
        foreach (var button in unlockedButtons)
        {
            Destroy(button.gameObject);
        }
        unlockedButtons.Clear();

        DislplayLevels();
    }

    public void OpenPopUp(LevelData ld)
    {
        selectedLevel = ld;
        StartLevelPopUpText.text = $"Do you want to play {ld.LevelName}?";
        StartLevelPopUp.gameObject.SetActive(true);
    }

    public void StartButton()
    {
        //Debug.Log(selectedLevel.LevelName);
        GameConnection.Instance.LevelSelectedAndStartPressed(selectedLevel);
    }

    public void CancelButton() 
    {
        selectedLevel = null;
        StartLevelPopUp.gameObject.SetActive(false);
    }

    void DislplayLevels()
    {
        int levelsUnlocked = GameConnection.Instance.LevelsUnlocked;
        if (levelsUnlocked > levelsAvailable.Count)
        {
            throw new System.ArgumentOutOfRangeException("levelsUnlocked", "More levels have been unlocked than are in total available!");
        }

        for (int i = 0; i < levelsUnlocked; i++)
        {
            LevelSelectButton lsb = Instantiate(levelButtonPrefab);
            lsb.Instantiate(levelsAvailable[i], this);

            if (map.getLevelPosition(i) == null)
            {
                Debug.LogError($"Map has no position assigned for level {i}");
                return;
            }
            lsb.transform.SetParent(map.getLevelPosition(i));
            lsb.transform.localPosition = Vector3.zero;
            unlockedButtons.Add(lsb);
        }
    }
}
