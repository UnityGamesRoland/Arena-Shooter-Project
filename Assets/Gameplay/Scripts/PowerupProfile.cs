using UnityEngine;

[CreateAssetMenu(fileName = "Powerup Profile", menuName = "Powerup Profile")]
public class PowerupProfile : SingletonScriptableObject<PowerupProfile>
{
    public float dashEnergyConsumption = 40f;
    public float dashLength = 0.06f;
    public float dashIFrameDuration = 0.5f;

    public float shieldMaintenanceEnergyConsumption = 3.5f;
    public float shieldBlockingEnergyConsumption = 25f;
    public float shieldActivationEnergyRequirement = 20f;
    public float shieldReflectBullets = 0f;
}
