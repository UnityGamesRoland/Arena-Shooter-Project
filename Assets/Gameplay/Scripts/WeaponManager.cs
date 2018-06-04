using UnityEngine;
using EZCameraShake;

public class WeaponManager : MonoBehaviour
{
    public WeaponBullet bulletPrefab;
    public Transform muzzleTransform;
    public AudioClip shootSound;
    public float fireRate = 0.1f;
    public float spread = 1.2f;

    private float shootTimer;
    private ParticleSystem muzzleFlash;
    private AudioSource source;
    private PlayerManager player;

    #region Singleton and References
    public static WeaponManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        muzzleFlash = muzzleTransform.GetComponent<ParticleSystem>();
        source = muzzleTransform.GetComponent<AudioSource>();
    }
    #endregion

    private void Start()
    {
        //Initialization.
        player = PlayerManager.Instance;
    }

    private void Update()
    {
        //Check if the player is alive.
        if (player.isDead) return;

        //Shoot while holding LMB.
        if(Input.GetMouseButton(0) && Time.time > shootTimer && Time.time > player.actionTimer) Shoot();
    }

    private void Shoot()
    {
        //Calculate the spread amount.
        Vector2 randomizedSpread = Random.insideUnitCircle * spread;

        //Instantiate the bullet at the muzzle and apply the spread on it's rotation.
        Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.Euler(transform.eulerAngles + new Vector3(randomizedSpread.x, randomizedSpread.y, 0)));

        //Shake the camera.
        CameraShaker.Instance.ShakeOnce(0.5f, 1.7f, 0.1f, 0.2f);

        //Play the muzzle flash particle.
        muzzleFlash.Play();

        //Play the shoot sound.
        source.pitch = Random.Range(0.97f, 1f);
        source.PlayOneShot(shootSound);

        //Update the action timer.
        shootTimer = Time.time + fireRate;
    }
}
