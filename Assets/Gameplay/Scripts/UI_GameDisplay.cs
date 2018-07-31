using UnityEngine;
using UnityEngine.UI;

public class UI_GameDisplay : MonoBehaviour
{
    public CanvasGroup dash;
    public CanvasGroup energyShield;
    public Image energyBar;
    public Image energyGlow;
    public Image dashCooldownFill;
    public Image shieldCooldownFill;
    public Image shieldActivityOverlay;

    #region Singleton
    public static UI_GameDisplay Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    #endregion

    public void UpdateEnergyBar(float value)
    {
        energyBar.fillAmount = value;
        energyGlow.fillAmount = value;
    }

    public void UpdateAbilityEnergyRequirementDisplay(bool canDash, bool canShield)
    {
        dash.alpha = canDash ? 1 : 0.25f;

        if (AbilityManager.Instance.isShielded)
        {
            energyShield.alpha = 1;
            shieldActivityOverlay.enabled = true;
        }

        else
        {
            energyShield.alpha = canShield ? 1 : 0.25f;
            shieldActivityOverlay.enabled = false;
        }
        
    }

    public void UpdateAbilityCooldownDisplay(float dashCooldown, float shieldCooldown)
    {
        if (dashCooldown > 0) dash.alpha = 0.25f;
        if (shieldCooldown > 0) energyShield.alpha = 0.25f;
        
        dashCooldownFill.fillAmount = 1 - dashCooldown;
        shieldCooldownFill.fillAmount = AbilityManager.Instance.isShielded ? 1 : 1 - shieldCooldown;
    }
}
