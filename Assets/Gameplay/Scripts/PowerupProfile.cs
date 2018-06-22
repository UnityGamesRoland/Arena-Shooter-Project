using UnityEngine;

[CreateAssetMenu(fileName = "Powerup Profile", menuName = "Powerup Profile")]
public class PowerupProfile : SingletonScriptableObject<PowerupProfile>
{
    public float dashLength = 0.06f;
    public float invincibilityDuration = 1f;

    public float grenadeRadius = 2.3f;
    public float grenadeDetonateTime = 1.5f;

    public string[] spawnablePowerups;
}
