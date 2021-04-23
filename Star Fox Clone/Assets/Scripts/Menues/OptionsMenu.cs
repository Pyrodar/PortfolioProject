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
    [SerializeField] AudioMixerGroup masterMixer;
    [SerializeField] AudioMixerGroup musicMixer;
    [SerializeField] AudioMixerGroup sfxMixer;
    [SerializeField] AudioMixerGroup voiceMixer;
    [SerializeField] AudioSource testSoundSource;

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
        currentSetup.Quality = QualitySettings.GetQualityLevel();

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
        currentSetup.Resolution = currentRes;

        #endregion

        #region fullscreenModeDropdown

        List<string> fullOptions = new List<string>();
        foreach (var mode in Enum.GetNames(typeof(FullScreenMode)))
        {
            fullOptions.Add(mode);
        }
        fullscreenModeDropdown.AddOptions(fullOptions);
        fullscreenModeDropdown.SetValueWithoutNotify((int)Screen.fullScreenMode);
        currentSetup.Fullscreen = Screen.fullScreenMode;

        #endregion




        VideoPanel.SetActive(false);
        #endregion


        #region Audio Settings

        AudioPanel.SetActive(true);

        float temp;

        audioMixer.GetFloat("MasterVolume", out temp);
        MasterVolumeSlider.maxValue = 20f;
        MasterVolumeSlider.minValue = -60f;
        MasterVolumeSlider.SetValueWithoutNotify(temp);

        audioMixer.GetFloat("MusicVolume", out temp);
        MusicVolumeSlider.maxValue = 20f;
        MusicVolumeSlider.minValue = -60f;
        MusicVolumeSlider.SetValueWithoutNotify(temp);

        audioMixer.GetFloat("SFXVolume", out temp);
        SFXVolumeSlider.maxValue = 20f;
        SFXVolumeSlider.minValue = -60f;
        SFXVolumeSlider.SetValueWithoutNotify(temp);

        audioMixer.GetFloat("VoiceVolume", out temp);
        VoiceVolumeSlider.maxValue = 20f;
        VoiceVolumeSlider.minValue = -60f;
        VoiceVolumeSlider.SetValueWithoutNotify(temp);

        #endregion
        nextSetup.CopyValues(currentSetup);
    }

    /// <summary>
    /// to be called when a new Setup shall be loaded
    /// </summary>
    /// <param name="setup"></param>
    public void loadSetup(OptionsSetup setup)
    {
        #region VideoSettings

        qualityDropDown.SetValueWithoutNotify(setup.Quality);
        resolutionDropdown.SetValueWithoutNotify(setup.Resolution);
        fullscreenModeDropdown.SetValueWithoutNotify((int)setup.Fullscreen);

        QualitySettings.SetQualityLevel(setup.Quality);

        int x = Screen.resolutions[setup.Resolution].width;
        int y = Screen.resolutions[setup.Resolution].height;
        Screen.SetResolution(x, y, setup.Fullscreen);

        #endregion
    }

    #region dropdowns
    public void QualityChanged(int dropValue)
    {
        Debug.Log("Quality changed: " + QualitySettings.names[dropValue]);

        nextSetup.Quality = dropValue;
    }

    public void ResolutionChanged(int dropValue)
    {
        Debug.Log("Resolution changed: " + Screen.resolutions[dropValue].ToString());

        nextSetup.Resolution = dropValue;
    }

    public void FullscreenModeChanged(int dropValue)
    {
        Debug.Log("FullscreenMode changed: " + Enum.GetName(typeof(FullScreenMode), dropValue));
        nextSetup.Fullscreen = (FullScreenMode)dropValue;
    }
    #endregion

    #region sliders

    public void MasterVolumeChanged(float value)
    {
        Debug.Log("MasterVol changed");
        audioMixer.SetFloat("MasterVolume", value);
        playTestSound(0);
    }
    public void SFXVolumeChanged(float value)
    {
        Debug.Log("SFXVol changed");
        audioMixer.SetFloat("SFXVolume", value);
        playTestSound(1);
    }

    public void MusicVolumeChanged(float value)
    {
        Debug.Log("MusicVol changed");
        audioMixer.SetFloat("MusicVolume", value);
        playTestSound(2);
    }

    public void VoiceVolumeChanged(float value)
    {
        Debug.Log("VoiceVol changed");
        audioMixer.SetFloat("VoiceVolume", value);
        playTestSound(3);
    }

    void playTestSound(int mixerGroup)
    {
        switch (mixerGroup)
        {
            case 0:     //Master
                testSoundSource.outputAudioMixerGroup = masterMixer;
                break;
            case 1:     //SFX
                testSoundSource.outputAudioMixerGroup = sfxMixer;
                break;
            case 2:     //Music
                testSoundSource.outputAudioMixerGroup = musicMixer;
                break;
            case 3:     //Voice
                testSoundSource.outputAudioMixerGroup = voiceMixer;
                break;
        }
        testSoundSource.Play();
    }
    #endregion

    #region ConfirmPopUp
    /// <summary>
    /// Applies the new setup, opens the ConfirmationPopup and starts the reset timer
    /// </summary>
    public void ApplyButtonPressed()
    {
        if (nextSetup.Equals(currentSetup)) return;
        loadSetup(nextSetup);
        openPopUp();
    }
    /// <summary>
    /// stops the timer and saves the new setup
    /// </summary>
    public void ConfirmButtonPressed()
    {
        currentSetup.CopyValues(nextSetup);
        cancelTimer();
    }
    /// <summary>
    /// resets the setup to the previously saved one after 10 seconds
    /// </summary>
    public void CancelNewSetup()
    {
        loadSetup(currentSetup);
        nextSetup.CopyValues(currentSetup);
        cancelTimer();
    }

    #region Timer
    float endTime = 0;

    void openPopUp()
    {
        ConfirmPopUp.SetActive(true);
        endTime = Time.time + 10f;
    }

    void cancelTimer()
    {
        endTime = 0;
        ConfirmPopUp.SetActive(false);
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
    public FullScreenMode Fullscreen;
    public int Quality;
    public int Resolution;

    public bool Equals(OptionsSetup other)
    {
        Debug.Log("Comparing settings:");
        if (other.Fullscreen == Fullscreen
            && other.Quality == Quality
            && other.Resolution == Resolution
            )
        {
            Debug.Log("Identical");
            return true;
        }

        Debug.Log("Different");
        return false;
    }

    public void CopyValues(OptionsSetup setup)
    {
        Fullscreen = setup.Fullscreen;
        Quality = setup.Quality;
        Resolution = setup.Resolution;
    }
}
