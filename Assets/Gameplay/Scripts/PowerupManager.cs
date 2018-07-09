using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour
{
    public PowerupProfile powerupStats;
    public GameObject energyShieldObject;

    public float maxEnergy = 100;
    public float currentEnergy = 100;
    public bool isShielded;

    private float energyRegenTimer;

    #region Singleton And References
    public static PowerupManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        energyShieldObject.SetActive(false);
    }
    #endregion

    private void Update()
    {
        UpdateEnergy();
        GetAbilityInput();
        HandleEnergyShield();

        UI_GameDisplay.Instance.UpdateEnergySlider(currentEnergy / maxEnergy);
    }

    private void UpdateEnergy()
    {
        if(!isShielded && !TP_Motor.Instance.states.isDashing)
        {
            if(Time.time > energyRegenTimer)
            {
                if (currentEnergy < maxEnergy)
                {
                    currentEnergy += Time.deltaTime * 15;
                    currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
                }
            }
        }
    }

    private void GetAbilityInput()
    {
        if (TP_Utilities.GetAction("Dash", KeybindingsProfile.Instance) && !PlayerManager.Instance.isDead)
        {
            if (currentEnergy >= powerupStats.dashEnergyConsumption)
            {
                if(!isShielded)
                {
                    TP_Motor.Instance.BeginDash(powerupStats.dashLength, powerupStats.dashIFrameDuration);
                    currentEnergy -= powerupStats.dashEnergyConsumption;
                    energyRegenTimer = Time.time + powerupStats.dashLength + 0.3f;
                }
            }
        }

        if (TP_Utilities.GetAction("ActivateShield", KeybindingsProfile.Instance) && !PlayerManager.Instance.isDead)
        {
            if (currentEnergy >= powerupStats.shieldActivationEnergyRequirement)
            {
                energyShieldObject.SetActive(true);
                isShielded = true;
            }
        }

        if (TP_Utilities.GetAction("DeactivateShield", KeybindingsProfile.Instance))
        {
            energyShieldObject.SetActive(false);
            isShielded = false;
            energyRegenTimer = Time.time + 0.3f;
        }
    }

    private void HandleEnergyShield()
    {
        if (currentEnergy < powerupStats.shieldMaintenanceEnergyConsumption || PlayerManager.Instance.isDead) isShielded = false;

        if (isShielded)
        {
            currentEnergy -= Time.deltaTime * powerupStats.shieldMaintenanceEnergyConsumption;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

            if(currentEnergy == 0)
            {
                energyShieldObject.SetActive(false);
                energyRegenTimer = Time.time + 0.6f;
                isShielded = false;
            }
        }
    }

    public void ShieldBlockDamage()
    {
        currentEnergy -= powerupStats.shieldBlockingEnergyConsumption;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        if (currentEnergy == 0)
        {
            energyShieldObject.SetActive(false);
            energyRegenTimer = Time.time + 0.6f;
            isShielded = false;
        }
    }
}
