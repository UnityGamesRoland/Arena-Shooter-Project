using UnityEngine;

public class ObstacleHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        //Apply damage on the player.
        PlayerManager.Instance.ApplyDamage(100, transform.position);
    }
}
