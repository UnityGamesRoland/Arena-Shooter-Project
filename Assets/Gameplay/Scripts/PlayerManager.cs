using UnityEngine;
using System.Collections;
using EZCameraShake;

public class PlayerManager : MonoBehaviour
{
    public ParticleSystem deathParticle;
    public Texture2D normalCrosshair;
    public Texture2D smallCrosshair;

    [HideInInspector] public float actionTimer;
    [HideInInspector] public bool isInvincible;
    [HideInInspector] public bool isDead;

    private Rigidbody[] ragdollParts;
    private int health = 8;

    private float invincibilityDuration;
    private float invincibilityTimer;

    #region Singleton And References
    public static PlayerManager Instance { get; private set; }
    private void Awake()
    {
        //Assign singleton instance.
        if (Instance == null) Instance = this;

        //Get the ragdoll parts and make them kinematic.
        ragdollParts = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in ragdollParts) body.isKinematic = true;
    }
    #endregion

    private void Update()
    {
        //Update the invinciblity.
        UpdateInvincibility();
    }

    public void ApplyDamage(int damage, Vector3 hitPoint)
    {
        //Can't take damage while any of these states active.
        if (isInvincible) return;

        //Check if the player is alive.
        if (health > 0)
        {
            //Apply the damage on the player.
            health -= damage;

            //Shake the camera.
            CameraShaker.Instance.ShakeOnce(1, 2.5f, 0.5f, 1f);

            //Check if the player died.
            if (health <= 0)
            {
                StartCoroutine(DestroyPlayer(hitPoint));
            }
        }
    }

    public void ApplyInvincibility(float duration)
    {
        //Add the duration to the timer.
        invincibilityDuration += duration;
    }

    private void UpdateInvincibility()
    {
        //Check if the player is invincible.
        if (invincibilityDuration > 0 && Time.time > invincibilityTimer)
        {
            //Update the timer.
            invincibilityTimer = Time.time + 0.5f;

            //Decrease the invincibility duration and clamp it.
            invincibilityDuration -= 0.5f;
            invincibilityDuration = Mathf.Clamp(invincibilityDuration, 0f, 100f);

            //Update the invincibility state.
            isInvincible = invincibilityDuration == 0 ? false : true;
        }
    }

    private IEnumerator DestroyPlayer(Vector3 hitPoint)
    {
        //Set the dying state, and prepare the ragdoll.
        isDead = true;
        TP_Animations.Instance.animator.enabled = false;

        //Enable physincs on the ragdoll parts.
        foreach (Rigidbody body in ragdollParts) body.isKinematic = false;

        //Apply an impulse on the ragdoll and unparent the rifle.
        ragdollParts[0].AddExplosionForce(240, hitPoint, 1, 0.3f, ForceMode.Impulse);
        ragdollParts[ragdollParts.Length - 1].transform.parent = null;

        //Play the death particle.
        deathParticle.Play();

        //Wait for the frame to end.
        yield return new WaitForSeconds(1.4f);

        //Restart the level.
        UI_SceneManager.Instance.RestartLevel();
    }
}
