using UnityEngine;
using System.Collections;

public class Powerup_Grenade : MonoBehaviour
{
    public Transform explosionMarker;
    public Transform explosionEffect;
    public LayerMask enemyLayer;

    public void DetonateGrenade(GameObject grenade, float detonateTime, float radius)
    {
        StartCoroutine(DetonateAfterTime(grenade, detonateTime, radius));
    }

    private IEnumerator DetonateAfterTime(GameObject grenade, float detonateTime, float radius)
    {
        //Since this is a circle, it doesn't matter which scale we comparing to.
        while(explosionMarker.localScale.x < radius)
        {
            float currentRadius = Mathf.MoveTowards(explosionMarker.localScale.x, radius, Time.deltaTime / 0.03f);
            explosionMarker.localScale = Vector3.one * currentRadius;

            yield return null;
        }

        yield return new WaitForSeconds(detonateTime);

        Collider[] enemiesInRange = Physics.OverlapSphere(grenade.transform.position, radius, enemyLayer, QueryTriggerInteraction.Collide);

        if (enemiesInRange.Length > 0)
        {
            foreach (Collider enemy in enemiesInRange)
            {
                if(enemy.tag == "Enemy") enemy.GetComponent<AI_Runner>().ApplyDamage(12, grenade.transform.position);
            }
        }

        Instantiate(explosionEffect, transform.position, explosionEffect.rotation);

        Destroy(grenade);
    }
}
