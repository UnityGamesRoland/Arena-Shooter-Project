using UnityEngine;

[CreateAssetMenu(fileName = "Settings Profile", menuName = "Settings Profile")]
public class SettingsProfile : SingletonScriptableObject<SettingsProfile>
{
    public int textureQuality = 1;
    public int resolutionScale = 5;
    public int multiSampling = 3;
    public int antiAliasing = 2;
    public int vSync = 0;

    public float masterVolume = 0.7f;
    public float musicVolume = 1f;
    public float gameplayVolume = 1f;

    [HideInInspector] public string[] textureQualityStrings = new string[] { "Low", "High" };
    [HideInInspector] public string[] resolutionScaleStrings = new string[] { "50%", "60%", "70%", "80%", "90%", "100%" };
    [HideInInspector] public string[] multiSamplingStrings = new string[] { "Disabled", "2x", "4x", "8x" };
    [HideInInspector] public string[] antiAliasingStrings = new string[] { "Disabled", "Low", "Medium", "High" };
    [HideInInspector] public string[] vSyncStrings = new string[] { "Disabled", "Enabled" };

    [HideInInspector] public int[] textureQualityValues = new int[] { 1, 0 };
    [HideInInspector] public float[] resolutionScaleValues = new float[] { 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f };
    [HideInInspector] public int[] multiSamplingValues = new int[] { 0, 2, 4, 8 };
    [HideInInspector] public int[] vSyncValues = new int[] { 0, 1 };

    public void ClampSettings()
    {
        textureQuality = Mathf.Clamp(textureQuality, 0, 1);
        resolutionScale = Mathf.Clamp(resolutionScale, 0, 5);
        multiSampling = Mathf.Clamp(multiSampling, 0, 3);
        antiAliasing = Mathf.Clamp(antiAliasing, 0, 3);
        vSync = Mathf.Clamp(vSync, 0, 1);

        masterVolume = Mathf.Clamp01(masterVolume);
        musicVolume = Mathf.Clamp01(musicVolume);
        gameplayVolume = Mathf.Clamp01(gameplayVolume);

        masterVolume = (float)System.Math.Round(masterVolume, 2);
        musicVolume = (float)System.Math.Round(musicVolume, 2);
        gameplayVolume = (float)System.Math.Round(gameplayVolume, 2);
    }
}
