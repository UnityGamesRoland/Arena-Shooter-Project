using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour
{
    public PowerupProfile powerupStats;
    public string currentPowerup;
    public Powerup_Grenade grenadePrefab;

    #region Singleton
    public static PowerupManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    #endregion

    private void Update()
    {
        if (TP_Utilities.GetAction("UsePowerup", KeybindingsProfile.Instance) && !PlayerManager.Instance.isDead)
        {
            UsePowerup(currentPowerup);
            //currentPowerup = "";
        }
    }

    private void UsePowerup(string powerup)
    {
        if (powerup == "") return;

        if(powerup == "Dash")
        {
            TP_Motor.Instance.BeginDash(powerupStats.dashLength, powerupStats.invincibilityDuration);
        }

        if(powerup == "Grenade")
        {
            Powerup_Grenade grenade = Instantiate(grenadePrefab, TP_Motor.Instance.data.mousePoint - Vector3.up * 1.2f, transform.rotation) as Powerup_Grenade;
            grenade.DetonateGrenade(grenade.gameObject, powerupStats.grenadeDetonateTime, powerupStats.grenadeRadius);
        }
    }

    
}
