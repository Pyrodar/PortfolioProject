using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : MonoBehaviour
{

    #region Singleton
    public static GameSettings instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one 'GameSettings' Instance!!!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    public delegate void OnSettingsChanged();
    public OnSettingsChanged onSettingsChanged;

    [SerializeField] AudioMixer mainAudioMixer;

    public void applySettings(OptionsSetup setup)
    {
        onSettingsChanged?.Invoke();
        mainAudioMixer.SetFloat("MasterVolume", setup.MasterVolume);
        mainAudioMixer.SetFloat("MusicVolume", setup.MusicVolume);
        mainAudioMixer.SetFloat("SFXVolume", setup.SFXVolume);

        //Screen.fullScreen = Fullscreen;
    }
}
