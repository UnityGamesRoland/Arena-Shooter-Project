using UnityEngine;
using System.Collections;
using EZCameraShake;

public class PistolManager : MonoBehaviour
{
    public PistolAsset currentPistol;
    public Transform muzzleTransform;

    private float aimTimer;
    private float shootTimer;

    private ParticleSystem muzzleFlash;
    private AudioSource source;

    private PlayerManager player;
    private PauseManager pause;
    private AbilityManager ability;
    private TP_Animations anim;

    #region Singleton and References
    public static PistolManager Instance { get; private set; }
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
        pause = PauseManager.Instance;
        ability = AbilityManager.Instance;
        anim = TP_Animations.Instance;
    }

    private void Update()
    {
        //Check if the player is alive.
        if (player.isDead || pause.isPaused) return;

        //Aim
        if(anim.isAiming)
        {
            if(Time.time > aimTimer) anim.isAiming = false;
        }

        //Shoot while holding LMB.
        if (Input.GetMouseButtonDown(1) && Time.time > shootTimer && Time.time > player.actionTimer && !ability.isShielded)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        if (!anim.isAiming) anim.EnterAimState();
        aimTimer = Time.time + 3.5f;

        yield return null;

        //Calculate the spread amount.
        Vector2 randomizedSpread = Random.insideUnitCircle * currentPistol.spread;

        //Instantiate the bullet at the muzzle and apply the spread on it's rotation.
        Instantiate(currentPistol.bulletPrefab, muzzleTransform.position, Quaternion.Euler(transform.eulerAngles + new Vector3(randomizedSpread.x, randomizedSpread.y, 0)));

        //Shake the camera.
        CameraShaker.Instance.ShakeOnce(0.5f, 1.7f, 0.1f, 0.2f);

        //Play the muzzle flash particle.
        muzzleFlash.Play();

        //Play the shoot sound.
        source.pitch = Random.Range(0.97f, 1f);
        source.PlayOneShot(currentPistol.shootSound);

        //Update the action timer.
        shootTimer = Time.time + currentPistol.fireRate;
    }
}
