//Based on the Video "How to make a LOADING BAR in Unity" by Brackeys
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingScreen : MonoBehaviour
{
    public Slider LoadingBar;

    private void Start()
    {
        if (GameStateConnection.Instance == null)
        {
            Debug.LogError("no GameStateConnection Object Found");
            LoadScene(1);
        }
        else
        {
            LoadScene(GameStateConnection.Instance.LevelToLoad);
        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        yield return new WaitForSeconds(0.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            LoadingBar.value = operation.progress;
            yield return null;
        }
    }
}
