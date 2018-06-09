using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_GraphicsManager : MonoBehaviour
{
    public GraphicsProfile graphics;
    public Text textureQualityText;
    public Text resolutionScaleText;
    public Text multiSamplingText;
    public Text vSyncText;

    private void Awake()
    {
        //Should only be called from the splash screen in the menu.
        StartCoroutine(InitializeGraphics());
    }

    public void OnEnterGraphicsMenu()
    {
        //Update display texts upon entering the menu.
        textureQualityText.text = graphics.textureQualityStrings[graphics.textureQuality];
        resolutionScaleText.text = graphics.resolutionScaleStrings[graphics.resolutionScale];
        multiSamplingText.text = graphics.multiSamplingStrings[graphics.multiSampling];
        vSyncText.text = graphics.vSyncStrings[graphics.vSync];
    }

    public void SetTextureQuality(bool isIncrease)
    {
        //Get the target value.
        int target = graphics.textureQuality;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        graphics.textureQuality = target;
        graphics.ClampGraphicsProfile();

        //Update the display text.
        textureQualityText.text = graphics.textureQualityStrings[graphics.textureQuality];

        //Apply the setting.
        QualitySettings.masterTextureLimit = graphics.textureQualityValues[graphics.textureQuality];
    }

    public void SetResolutionScale(bool isIncrease)
    {
        //Get the target value.
        int target = graphics.resolutionScale;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        graphics.resolutionScale = target;
        graphics.ClampGraphicsProfile();

        //Update the display text.
        resolutionScaleText.text = graphics.resolutionScaleStrings[graphics.resolutionScale];

        //Apply the settings.
        QualitySettings.resolutionScalingFixedDPIFactor = graphics.resolutionScaleValues[graphics.resolutionScale];
    }

    public void SetMultiSampling(bool isIncrease)
    {
        //Get the target value.
        int target = graphics.multiSampling;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        graphics.multiSampling = target;
        graphics.ClampGraphicsProfile();

        //Update the display text.
        multiSamplingText.text = graphics.multiSamplingStrings[graphics.multiSampling];

        //Apply the settings.
        QualitySettings.antiAliasing = graphics.multiSamplingValues[graphics.multiSampling];
    }

    public void SetVSync(bool isIncrease)
    {
        //Get the target value.
        int target = graphics.vSync;

        //Update the target value.
        if (isIncrease) target++;
        else target--;

        //Set the appropriate setting, and clamp the value.
        graphics.vSync = target;
        graphics.ClampGraphicsProfile();

        //Update the display text.
        vSyncText.text = graphics.vSyncStrings[graphics.vSync];

        //Apply the settings.
        QualitySettings.vSyncCount = graphics.vSyncValues[graphics.vSync];
    }

    public IEnumerator InitializeGraphics()
    {
        //Apply the settings.
        QualitySettings.masterTextureLimit = graphics.textureQualityValues[graphics.textureQuality];
        QualitySettings.resolutionScalingFixedDPIFactor = graphics.resolutionScaleValues[graphics.resolutionScale];

        //Break point.
        yield return null;

        QualitySettings.antiAliasing = graphics.multiSamplingValues[graphics.multiSampling];
        QualitySettings.vSyncCount = graphics.vSyncValues[graphics.vSync];
    }
}