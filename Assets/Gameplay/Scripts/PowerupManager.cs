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
        if(!PlayerManager.Instance.isDead)
        {
            UpdateEnergy();
            GetAbilityInput();
            HandleEnergyShield();

            UI_GameDisplay.Instance.UpdateEnergySlider(currentEnergy / maxEnergy);
            return;
        }

        if (isShielded)
        {
            energyShieldObject.SetActive(false);
            isShielded = false;
        }
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
        if (TP_Utilities.GetAction("Dash", KeybindingsProfile.Instance))
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

        if (TP_Utilities.GetAction("ActivateShield", KeybindingsProfile.Instance))
        {
            if (currentEnergy >= powerupStats.shieldActivationEnergyRequirement)
            {
                energyShieldObject.SetActive(true);
                isShielded = true;
            }
        }

        if (TP_Utilities.GetAction("DeactivateShield", KeybindingsProfile.Instance))
        {
            if(isShielded)
            {
                energyRegenTimer = Time.time + 0.3f;
                isShielded = false;
            }
        }
    }

    private void HandleEnergyShield()
    {
        if (isShielded)
        {
            energyShieldObject.transform.localScale = Vector3.Lerp(energyShieldObject.transform.localScale, Vector3.one, Time.deltaTime * 10f);

            currentEnergy -= Time.deltaTime * powerupStats.shieldMaintenanceEnergyConsumption;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

            if(currentEnergy == 0)
            {
                energyRegenTimer = Time.time + 0.6f;
                isShielded = false;
            }
        }

        else
        {
            energyShieldObject.transform.localScale = Vector3.Lerp(energyShieldObject.transform.localScale, Vector3.one * 0.5f, Time.deltaTime * 10f);
            if (energyShieldObject.transform.localScale.x < 0.58f) energyShieldObject.SetActive(false);
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
