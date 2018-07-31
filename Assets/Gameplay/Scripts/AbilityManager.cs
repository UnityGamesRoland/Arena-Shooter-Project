using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public AbilityProfile abilityStats;
    public GameObject energyShieldObject;
    public GameObject orbPrefab;

    public float maxEnergy = 100;
    public float currentEnergy = 100;
    public float energyRegenSpeed = 17;
    public bool isShielded;

    private float energyRegenTimer;
    private float dashCooldownTimer;
    private float shieldCooldownTimer;

    #region Singleton And References
    public static AbilityManager Instance;
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
            UpdateCooldowns();
            UpdateAbilities();

            float dashCooldown = dashCooldownTimer == 0 ? 0 : dashCooldownTimer / abilityStats.dashCooldownTime;
            float shieldCooldown = shieldCooldownTimer == 0 ? 0 : shieldCooldownTimer / abilityStats.shieldCooldownTime;

            UI_GameDisplay.Instance.UpdateAbilityCooldownDisplay(dashCooldown, shieldCooldown);
            UI_GameDisplay.Instance.UpdateEnergyBar(currentEnergy / maxEnergy);
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
                    currentEnergy += Time.deltaTime * energyRegenSpeed;
                    currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
                }
            }
        }
    }
    
    private void UpdateCooldowns()
    {
        if(dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer < 0) dashCooldownTimer = 0;
        }

        if (shieldCooldownTimer > 0)
        {
            shieldCooldownTimer -= Time.deltaTime;
            if (shieldCooldownTimer < 0) shieldCooldownTimer = 0;
        }
    }

    private void UpdateAbilities()
    {
        bool enoughEnergyToDash = currentEnergy >= abilityStats.dashEnergyConsumption;
        bool enoughEnergyToShield = currentEnergy >= abilityStats.shieldActivationEnergyRequirement;

        UI_GameDisplay.Instance.UpdateAbilityEnergyRequirementDisplay(enoughEnergyToDash, enoughEnergyToShield);

        HandleDash(enoughEnergyToDash);
        HandleEnergyShield(enoughEnergyToShield);
        HandleOrbs(true);
    }

    private void HandleDash(bool hasEnoughEnergy)
    {
        if (TP_Utilities.GetAction("Dash", KeybindingsProfile.Instance))
        {
            if (hasEnoughEnergy)
            {
                if(dashCooldownTimer == 0)
                {
                    if (!isShielded)
                    {
                        TP_Motor.Instance.BeginDash(abilityStats.dashLength, abilityStats.dashIFrameDuration);
                        currentEnergy -= abilityStats.dashEnergyConsumption;

                        dashCooldownTimer = abilityStats.dashCooldownTime;
                        energyRegenTimer = Time.time + abilityStats.dashLength + 0.3f;
                    }
                }
            }
        }
    }

    private void HandleEnergyShield(bool hasEnoughEnergy)
    {
        if (TP_Utilities.GetAction("ActivateShield", KeybindingsProfile.Instance))
        {
            if (hasEnoughEnergy)
            {
                if(shieldCooldownTimer == 0)
                {
                    energyShieldObject.SetActive(true);
                    isShielded = true;
                }
            }
        }

        if (TP_Utilities.GetAction("DeactivateShield", KeybindingsProfile.Instance))
        {
            if (isShielded)
            {
                energyRegenTimer = Time.time + 0.3f;
                shieldCooldownTimer = abilityStats.shieldCooldownTime;
                isShielded = false;
            }
        }

        if (isShielded)
        {
            energyShieldObject.transform.localScale = Vector3.Lerp(energyShieldObject.transform.localScale, Vector3.one, Time.deltaTime * 10f);

            currentEnergy -= Time.deltaTime * abilityStats.shieldMaintenanceEnergyConsumption;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

            if(currentEnergy == 0)
            {
                energyRegenTimer = Time.time + 0.6f;
                shieldCooldownTimer = abilityStats.shieldCooldownTime;
                isShielded = false;
            }
        }

        else
        {
            energyShieldObject.transform.localScale = Vector3.Lerp(energyShieldObject.transform.localScale, Vector3.one * 0.5f, Time.deltaTime * 10f);
            if (energyShieldObject.transform.localScale.x < 0.58f) energyShieldObject.SetActive(false);
        }
    }

    private void HandleOrbs(bool hasEnoughEnergy)
    {
        if (TP_Utilities.GetAction("SpawnOrbs", KeybindingsProfile.Instance))
        {
            if(hasEnoughEnergy)
            {
                float orbCount = 3;
                float angle = 0;

                for (int i = 0; i < 3; i++)
                {
                    float posX = Mathf.Cos(angle * Mathf.Deg2Rad);
                    float posZ = Mathf.Sin(angle * Mathf.Deg2Rad);
                    angle += 360 / orbCount;

                    Vector3 offsetDir = transform.right * posX + transform.forward * posZ;
                    Vector3 spawnPoint = transform.position + offsetDir * 1.3f;

                    Transform orb = Instantiate(orbPrefab, spawnPoint, Quaternion.identity).transform;

                    orb.LookAt(transform.position);
                }
            }
        }
    }

    public void ShieldBlockDamage()
    {
        currentEnergy -= abilityStats.shieldBlockingEnergyConsumption;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        if (currentEnergy == 0)
        {
            energyShieldObject.SetActive(false);
            energyRegenTimer = Time.time + 0.6f;
            shieldCooldownTimer = abilityStats.shieldCooldownTime;
            isShielded = false;
        }
    }
}
