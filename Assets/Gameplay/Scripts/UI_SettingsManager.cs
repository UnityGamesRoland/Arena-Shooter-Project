using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UI_SettingsManager : MonoBehaviour
{
    public SettingsProfile settings;
    public AudioMixer mixer;

    public Text textureQualityText;
    public Text resolutionScaleText;
    public Text multiSamplingText;
    public Text antiAliasingText;
    public Text vSyncText;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider gameplayVolumeSlider;

    private void Start()
    {
        //Should only be called from the splash screen in the menu.
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            settings.ClampSettings();
            InitializeQualitySettings();
        }

        //Since the anti-aliasing is on a post processing layer, it has to be set every time a level loads.
        ApplyPerLevelSettings();
    }

    public void OnEnterGraphicsMenu()
    {
        //Update display texts upon entering the menu.
        textureQualityText.text = settings.textureQualityStrings[settings.textureQuality];
        resolutionScaleText.text = settings.resolutionScaleStrings[settings.resolutionScale];
        multiSamplingText.text = settings.multiSamplingStrings[settings.multiSampling];
        antiAliasingText.text = settings.antiAliasingStrings[settings.antiAliasing];
        vSyncText.text = settings.vSyncStrings[settings.vSync];
    }

    public void OnEnterSoundsMenu()
    {
        //Update the sliders.
        masterVolumeSlider.value = settings.masterVolume;
        musicVolumeSlider.value = settings.musicVolume;
        gameplayVolumeSlider.value = settings.gameplayVolume;

        //Update the volume levels.
        mixer.SetFloat("MasterVolume", Mathf.Lerp(-80f, 0f, settings.masterVolume));
        mixer.SetFloat("MusicVolume", Mathf.Lerp(-80f, 0f, settings.musicVolume));
        mixer.SetFloat("GameplayVolume", Mathf.Lerp(-80f, 0f, settings.gameplayVolume));
    }

    public void SetTextureQuality(bool isIncrease)
    {
        //Get the target value.
        int target = settings.textureQuality;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        settings.textureQuality = target;
        settings.ClampSettings();

        //Update the display text.
        textureQualityText.text = settings.textureQualityStrings[settings.textureQuality];

        //Apply the setting.
        QualitySettings.masterTextureLimit = settings.textureQualityValues[settings.textureQuality];
    }

    public void SetResolutionScale(bool isIncrease)
    {
        //Get the target value.
        int target = settings.resolutionScale;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        settings.resolutionScale = target;
        settings.ClampSettings();

        //Update the display text.
        resolutionScaleText.text = settings.resolutionScaleStrings[settings.resolutionScale];

        //Apply the settings.
        QualitySettings.resolutionScalingFixedDPIFactor = settings.resolutionScaleValues[settings.resolutionScale];
    }

    public void SetMultiSampling(bool isIncrease)
    {
        //Get the target value.
        int target = settings.multiSampling;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        settings.multiSampling = target;
        settings.ClampSettings();

        //Update the display text.
        multiSamplingText.text = settings.multiSamplingStrings[settings.multiSampling];

        //Apply the settings.
        QualitySettings.antiAliasing = settings.multiSamplingValues[settings.multiSampling];
    }

    public void SetAntiAliasing(bool isIncrease)
    {
        //Get the target value.
        int target = settings.antiAliasing;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        settings.antiAliasing = target;
        settings.ClampSettings();

        //Update the display text.
        antiAliasingText.text = settings.antiAliasingStrings[settings.antiAliasing];

        //Get the post processing layer from the main camera.
        PostProcessLayer postProcess = Camera.main.GetComponent<PostProcessLayer>();

        //Check if the anti-aliasing should be enabled.
        if (settings.antiAliasing == 0)
        {
            postProcess.antialiasingMode = PostProcessLayer.Antialiasing.None;
            return;
        }

        else postProcess.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;

        //Set the quality of the anti-aliasing.
        if (settings.antiAliasing == 1) postProcess.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Low;
        if (settings.antiAliasing == 2) postProcess.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Medium;
        if (settings.antiAliasing == 3) postProcess.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
    }

    public void SetVSync(bool isIncrease)
    {
        //Get the target value.
        int target = settings.vSync;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        settings.vSync = target;
        settings.ClampSettings();

        //Update the display text.
        vSyncText.text = settings.vSyncStrings[settings.vSync];

        //Apply the settings.
        QualitySettings.vSyncCount = settings.vSyncValues[settings.vSync];
    }

    public void SetMasterVolume(bool isIncrease)
    {
        //Update the slider's value. SetVolumeDirectly() will automatically be called since its connected to the OnValueChange() event of the slider.
        if (isIncrease) masterVolumeSlider.value += 0.01f;
        else masterVolumeSlider.value -= 0.01f;
    }

    public void SetMusicVolume(bool isIncrease)
    {
        //Update the slider's value. SetVolumeDirectly() will automatically be called since its connected to the OnValueChange() event of the slider.
        if (isIncrease) musicVolumeSlider.value += 0.01f;
        else musicVolumeSlider.value -= 0.01f;
    }

    public void SetGameplayVolume(bool isIncrease)
    {
        //Update the slider's value. SetVolumeDirectly() will automatically be called since its connected to the OnValueChange() event of the slider.
        if (isIncrease) gameplayVolumeSlider.value += 0.01f;
        else gameplayVolumeSlider.value -= 0.01f;
    }

    public void SetMasterVolumeDirectly()
    {
        //Set the appropriate setting, and clamp the value.
        settings.masterVolume = masterVolumeSlider.value;
        settings.ClampSettings();

        //Update the volume level.
        mixer.SetFloat("MasterVolume", Mathf.Lerp(-80f, 0f, settings.masterVolume));
    }

    public void SetMusicVolumeDirectly()
    {
        //Set the appropriate setting, and clamp the value.
        settings.musicVolume = musicVolumeSlider.value;
        settings.ClampSettings();

        //Update the volume level.
        mixer.SetFloat("MusicVolume", Mathf.Lerp(-80f, 0f, settings.musicVolume));
    }

    public void SetGameplayVolumeDirectly()
    {
        //Set the appropriate setting, and clamp the value.
        settings.gameplayVolume = gameplayVolumeSlider.value;
        settings.ClampSettings();

        //Update the volume level.
        mixer.SetFloat("GameplayVolume", Mathf.Lerp(-80f, 0f, settings.gameplayVolume));
    }

    private void ApplyPerLevelSettings()
    {
        //Update the volume levels.
        mixer.SetFloat("MasterVolume", Mathf.Lerp(-80f, 0f, settings.masterVolume));
        mixer.SetFloat("MusicVolume", Mathf.Lerp(-80f, 0f, settings.musicVolume));
        mixer.SetFloat("GameplayVolume", Mathf.Lerp(-80f, 0f, settings.gameplayVolume));

        //Get the post processing layer from the main camera.
        PostProcessLayer postProcess = Camera.main.GetComponent<PostProcessLayer>();

        //Check if the anti-aliasing should be enabled.
        if (settings.antiAliasing == 0)
        {
            postProcess.antialiasingMode = PostProcessLayer.Antialiasing.None;
            return;
        }

        else postProcess.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;

        //Set the quality of the anti-aliasing.
        if (settings.antiAliasing == 1) postProcess.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Low;
        if (settings.antiAliasing == 2) postProcess.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Medium;
        if (settings.antiAliasing == 3) postProcess.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
    }

    private void InitializeQualitySettings()
    {
        //Apply the quality settings.
        QualitySettings.masterTextureLimit = settings.textureQualityValues[settings.textureQuality];
        QualitySettings.resolutionScalingFixedDPIFactor = settings.resolutionScaleValues[settings.resolutionScale];
        QualitySettings.antiAliasing = settings.multiSamplingValues[settings.multiSampling];
        QualitySettings.vSyncCount = settings.vSyncValues[settings.vSync];
    }
}