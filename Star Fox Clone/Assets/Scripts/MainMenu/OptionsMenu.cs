using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class OptionsMenu : MonoBehaviour
{
    OptionsSetup nextSetup;
    OptionsSetup currentSetup;

    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Dropdown qualityDropDown;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Dropdown fullscreenModeDropdown;

    [SerializeField] GameObject VideoPanel;
    [SerializeField] GameObject AudioPanel;
    [SerializeField] GameObject ConfirmPopUp;

    [SerializeField] Slider MasterVolumeSlider;
    [SerializeField] Slider MusicVolumeSlider;
    [SerializeField] Slider SFXVolumeSlider;
    [SerializeField] Slider VoiceVolumeSlider;

    /// <summary>
    /// Listing all possible options and saving current setup
    /// </summary>
    private void Start()
    {
        nextSetup = new OptionsSetup();
        currentSetup = new OptionsSetup();

        #region Video settings
        VideoPanel.SetActive(true);

        #region QualityDropdown

        List<string> options = new List<string>();
        foreach (string item in QualitySettings.names)
        {
            options.Add(item);
        }

        qualityDropDown.AddOptions(options);
        qualityDropDown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        currentSetup.quality = QualitySettings.GetQualityLevel();

        #endregion

        #region resolutionDropdown
        List<string> resOptions = new List<string>();
        int currentRes = 0;
        int n = 0;
        foreach (var item in Screen.resolutions)
        {
            resOptions.Add(item.width + "x" + item.height);
            if (item.width == Screen.currentResolution.width) currentRes = n;
            n++;
        }

        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.SetValueWithoutNotify(currentRes);
        currentSetup.resolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);

        #endregion

        #region fullscreenModeDropdown

        List<string> fullOptions = new List<string>();
        foreach (var mode in Enum.GetNames(typeof(FullScreenMode)))
        {
            fullOptions.Add(mode);
        }
        fullscreenModeDropdown.AddOptions(fullOptions);
        fullscreenModeDropdown.SetValueWithoutNotify((int)Screen.fullScreenMode);
        currentSetup.fullscreen = Screen.fullScreenMode;

        #endregion




        VideoPanel.SetActive(false);
        #endregion


        #region Audio Settings

        AudioPanel.SetActive(true);

        float temp;

        audioMixer.GetFloat("MasterVolume", out temp);
        MasterVolumeSlider.SetValueWithoutNotify(temp);
        MasterVolumeSlider.maxValue = 20f;
        MasterVolumeSlider.minValue = -60f;
        currentSetup.MasterVolume = temp;

        audioMixer.GetFloat("MusicVolume", out temp);
        MusicVolumeSlider.SetValueWithoutNotify(temp);
        MusicVolumeSlider.maxValue = 20f;
        MusicVolumeSlider.minValue = -60f;
        currentSetup.MusicVolume = temp;

        audioMixer.GetFloat("SFXVolume", out temp);
        SFXVolumeSlider.SetValueWithoutNotify(temp);
        SFXVolumeSlider.maxValue = 20f;
        SFXVolumeSlider.minValue = -60f;
        currentSetup.SFXVolume = temp;

        audioMixer.GetFloat("VoiceVolume", out temp);
        VoiceVolumeSlider.SetValueWithoutNotify(temp);
        VoiceVolumeSlider.maxValue = 20f;
        VoiceVolumeSlider.minValue = -60f;
        currentSetup.VoiceVolume = temp;

        //AudioPanel.SetActive(false);

        #endregion

        gameObject.SetActive(false);
    }

    /// <summary>
    /// to be called when a new Setup shall be loaded
    /// </summary>
    /// <param name="setup"></param>
    public void loadSetup(OptionsSetup setup)
    {
        #region AudioSettings

        audioMixer.SetFloat("MasterVolume", setup.MasterVolume);
        audioMixer.SetFloat("MusicVolume", setup.MusicVolume);
        audioMixer.SetFloat("SFXVolume", setup.SFXVolume);
        audioMixer.SetFloat("VoiceVolume", setup.VoiceVolume);

        #endregion

        #region VideoSettings

        QualitySettings.SetQualityLevel(setup.quality);

        Screen.SetResolution(setup.resolution.x, setup.resolution.y, setup.fullscreen);


        #endregion
    }

    #region dropdowns
    public void QualityChanged(int dropValue)
    {
        Debug.Log("Quality changed: " + QualitySettings.names[dropValue]);

        nextSetup.quality = dropValue;
    }

    public void ResolutionChanged(int dropValue)
    {
        Debug.Log("Resolution changed: " + Screen.resolutions[dropValue].ToString());
        int x = Screen.resolutions[dropValue].width;
        int y = Screen.resolutions[dropValue].height;

        nextSetup.resolution = new Vector2Int(x, y);
    }

    public void FullscreenModeChanged(int dropValue)
    {
        Debug.Log("FullscreenMode changed: " + Enum.GetName(typeof(FullScreenMode), dropValue));
        nextSetup.fullscreen = (FullScreenMode)dropValue;
    }
    #endregion

    #region sliders

    public void MasterVolumeChanged(float value)
    {
        nextSetup.MasterVolume = value;
    }
    public void SFXVolumeChanged(float value)
    {
        nextSetup.SFXVolume = value;
    }

    public void MusicVolumeChanged(float value)
    {
        nextSetup.MusicVolume = value;
    }

    public void VoiceVolumeChanged(float value)
    {
        nextSetup.VoiceVolume = value;
    }

    #endregion

    #region ConfirmPopUp
    /// <summary>
    /// Applies the new setup, opens the ConfirmationPopup and starts the reset timer
    /// </summary>
    public void ApplyButtonPressed()
    {
        if (nextSetup == currentSetup) return;
        loadSetup(nextSetup);
        OpenPopUp();
    }
    /// <summary>
    /// stops the timer and saves the new setup
    /// </summary>
    public void ConfirmButtonPressed()
    {
        currentSetup = nextSetup;
        ConfirmPopUp.SetActive(false);
        cancelTimer();
    }
    /// <summary>
    /// resets the setup to the previously saved one after 10 seconds
    /// </summary>
    public void CancelNewSetup()
    {
        loadSetup(currentSetup);
        nextSetup = currentSetup;
        ConfirmPopUp.SetActive(false);
        cancelTimer();
    }

    #region Timer
    float endTime = 0;

    void OpenPopUp()
    {
        ConfirmPopUp.SetActive(true);
        endTime = Time.time + 10f;
    }

    void cancelTimer()
    {
        endTime = 0;
    }

    private void Update()
    {
        if (endTime != 0 && Time.time > endTime)
        {
            CancelNewSetup();
        }
    }
    #endregion

    #endregion
}

public class OptionsSetup : IEquatable<OptionsSetup>
{
    public FullScreenMode fullscreen;
    public int quality;
    public Vector2Int resolution;

    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;
    public float VoiceVolume;

    public bool Equals(OptionsSetup other)
    {
        if (other.fullscreen == fullscreen
            && other.quality == quality
            && other.resolution == resolution
            && other.MasterVolume == MasterVolume
            && other.MusicVolume == MusicVolume
            && other.SFXVolume == SFXVolume
            && other.VoiceVolume == VoiceVolume
            )
        {
            return true;
        }

        return false;
    }
}
