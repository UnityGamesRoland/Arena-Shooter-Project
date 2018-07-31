using UnityEngine;

[CreateAssetMenu(fileName = "New Pistol", menuName = "Pistol Asset")]
public class PistolAsset : ScriptableObject
{
    public WeaponBullet bulletPrefab;
    public AudioClip shootSound;
    public float fireRate = 0.1f;
    public float spread = 1.2f;
}
