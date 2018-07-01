using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour
{
    public PowerupProfile powerupStats;
    public string currentPowerup;
    public Transform explosionEffect;
    public LayerMask enemyLayer;

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
            Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, powerupStats.explosionRadius, enemyLayer, QueryTriggerInteraction.Collide);

            if (enemiesInRange.Length > 0)
            {
                foreach (Collider enemy in enemiesInRange)
                {
                    if (enemy.tag == "Enemy") enemy.GetComponent<AI_Runner>().ApplyDamage(12, transform.position);
                }
            }

            Instantiate(explosionEffect, transform.position, explosionEffect.rotation);
        }
    }

    
}
