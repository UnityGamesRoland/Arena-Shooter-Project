using UnityEngine;
using System.Collections;
using EZCameraShake;

public class TP_Motor : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 50f;

    public KeybindingsProfile keybindings;
    public ParticleSystem dashParticle;
    public AudioClip dashSound;
    public AudioClip[] footstepSounds;

    public CharacterStates states;
    public CharacterData data;

    private Vector3 moveInterpolationVelocity;
    private Plane rotationPlane;

    private AudioSource source;
    private PlayerManager player;
    private PauseManager pause;
    private TP_Animations anim;
    private UI_SceneManager sceneManager;

    #region Singleton and References
    public static TP_Motor Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        data.controller = GetComponent<CharacterController>();
        source = GetComponent<AudioSource>();
    }
    #endregion

    private void Start()
    {
        //Initialization.
        player = PlayerManager.Instance;
        anim = TP_Animations.Instance;
        pause = PauseManager.Instance;
        sceneManager = UI_SceneManager.Instance;

        //Create a virtual plane for the rotation.
        rotationPlane = new Plane(Vector3.up, Vector3.up * (transform.position.y + 0.465f));

        //Call the body rotation once to try matching the mouse and the crosshair position.
        RotateBody();

        //Repeatedly invoke the footstep sound player method.
        InvokeRepeating("PlayFootstepSound", 0f, 0.275f);
    }

    private void Update()
    {
        //Check if the scene has been initialized.
        if (!sceneManager.isInitialized) return;

        //Check if the game is paused.
        if(!pause.isPaused)
        {
            //Get the input direction and normalize it.
            data.inputDirection = new Vector2(TP_Utilities.GetAxis("Horizontal", keybindings), TP_Utilities.GetAxis("Vertical", keybindings)).normalized;

            //Stop the movement if the player is dead.
            if (player.isDead) data.inputDirection = Vector2.zero;

            //Move the player in the input direction if we are not dashing.
            if (!states.isDashing) Move(data.inputDirection, moveSpeed, false);

            //Rotate the player to mouse point.
            RotateBody();
        }
    }

    private void Move(Vector2 moveDir, float moveSpeed, bool rawVelocity)
    {
        //Update the movement state.
        states.isMoving = (moveDir.magnitude != 0) ? true : false;

        //Get the target move velocity.
        Vector3 targetVelocity = TP_Utilities.GetTargetMoveVelocity(moveDir, moveSpeed);

        //Interpolate and set the move velocity.
        data.moveVelocity = rawVelocity ? targetVelocity : Vector3.SmoothDamp(data.moveVelocity, targetVelocity, ref moveInterpolationVelocity, 0.05f);

        //Move the player.
        data.controller.Move(data.moveVelocity * Time.deltaTime);

        //Calculate the move magnitude after the player has moved.
        float moveMagnitude = new Vector2(data.controller.velocity.x, data.controller.velocity.z).magnitude / moveSpeed;

        //Animate the player.
        anim.AnimateCharacter(moveDir, moveMagnitude);
    }

    public void BeginDash(float dashLength, float invincibilityDuration)
    {
        //Pass in the direction and length to the dash handler enumerator and start it.
        StartCoroutine(Dash(dashLength, invincibilityDuration));
    }

    
    private IEnumerator Dash(float dashLength, float invincibilityDuration)
    {
        //Update the dashing state.
        states.isDashing = true;
        player.isInvincible = true;

        //Dash direction.
        Vector2 dashDir = data.inputDirection;

        //Use forward direction if no other direction were given.
        if (dashDir.magnitude == 0) dashDir = new Vector2(transform.forward.x, transform.forward.z).normalized;

        //Update the action timer. Delay required to compensate the fast movement.
        player.actionTimer = Time.time + dashLength + 0.17f;

        //Enable the dash FX.
        dashParticle.Play();
        source.PlayOneShot(dashSound, 0.6f);

        //Shake the camera.
        CameraShaker.Instance.ShakeOnce(1.3f, 1.6f, 0.1f, 0.5f);

        //Reset the progress.
        float progress = 0f;

        while (progress < 1)
        {
            //Update the progress.
            progress += Time.deltaTime / dashLength;

            //Move the player.
            Move(dashDir, dashSpeed, true);

            //Wait for the frame to end.
            yield return null;
        }

        //Shake the camera.
        CameraShaker.Instance.ShakeOnce(1.3f, 1.6f, 0.2f, 0.5f);

        //Apply invincibility on the player.
        player.ApplyInvincibility(invincibilityDuration);

        //Update the dashing state.
        states.isDashing = false;

        //Disable the dash FX after a bit of delay to let it catch up with the position.
        yield return new WaitForSeconds(0.08f);
        dashParticle.Stop();
    }

    private void RotateBody()
    {
        //Check if the player is dead.
        if (player.isDead) return;

        //Shoot a ray from the camera to the mouse position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        //Check where did the ray hit the plane.
        if (rotationPlane.Raycast(ray, out rayDistance))
        {
            //Get the mouse point.
            data.mousePoint = ray.GetPoint(rayDistance);

            if (Time.timeScale != 1)
            {
                //Get the look rotation and rotate the player to the mouse point.
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(data.mousePoint.x, transform.position.y, data.mousePoint.z) - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime / 0.025f);
            }

            else
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(data.mousePoint.x, transform.position.y, data.mousePoint.z) - transform.position);
                transform.rotation = lookRotation;
            }
        }
    }

    private void PlayFootstepSound()
    {
        //Check several conditions to see if a footstep sound should be played.
        if (states.isMoving && !states.isDashing && (data.controller.velocity.magnitude / moveSpeed) > 0.15f)
        {
            //Get a random sound from the array and play it.
            int randomIndex = Random.Range(0, footstepSounds.Length);
            source.PlayOneShot(footstepSounds[randomIndex], 0.05f);
        }
    }

    private void OnGUI()
    {
        //Check if the crosshair should be displayed.
        if (!sceneManager.isInitialized || sceneManager.isLoading || pause.isPaused) return;

        //Calculate the crosshair's position.
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(data.mousePoint);
        float screenPositionY = Screen.height - screenPosition.y;

        //Draw the crosshair.
        Rect screenCenter = new Rect(screenPosition.x - 16, screenPositionY - 16, 32, 32);
        GUI.DrawTexture(screenCenter, (Screen.width <= 1280) ? player.smallCrosshair : player.normalCrosshair);
    }
}
