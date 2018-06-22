using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public Transform powerupPrefab;
    public Vector3 areaCenter;
    public Vector3 areaSize;

    public void SpawnPowerup()
    {
        //...
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}
