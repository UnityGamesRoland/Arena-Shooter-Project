using UnityEngine;

[CreateAssetMenu(fileName = "Ability Profile", menuName = "Ability Profile")]
public class AbilityProfile : SingletonScriptableObject<AbilityProfile>
{
    public float dashEnergyConsumption = 40f;
    public float dashLength = 0.06f;
    public float dashIFrameDuration = 0.5f;
    public float dashCooldownTime = 0.1f;

    public float shieldMaintenanceEnergyConsumption = 3.5f;
    public float shieldBlockingEnergyConsumption = 25f;
    public float shieldActivationEnergyRequirement = 20f;
    public float shieldReflectBullets = 0f;
    public float shieldCooldownTime = 0.4f;
}
