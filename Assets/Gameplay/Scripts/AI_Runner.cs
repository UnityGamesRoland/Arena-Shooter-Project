using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AI_Runner : MonoBehaviour
{
    public Transform deathParticle;
    public float attackDistance;
    public float slashDistance;

    private int health = 2;
    private float attackTimer;

    private bool isInAttackRange;
    private bool isInitialized;
    private bool isRagdoll;
    private bool isDead;

    private Rigidbody[] ragdollParts;
    private NavMeshAgent agent;
    private Transform target;

    private Animator animator;
    private PlayerManager player;
    private TP_Motor motor;

    private void Awake()
    {
        //References.
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        //Get the ragdoll parts and make them kinematic.
        ragdollParts = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in ragdollParts) body.isKinematic = true;
    }

    private void Start()
    {
        //Initalization.
        motor = TP_Motor.Instance;
        player = PlayerManager.Instance;
        target = motor.transform;

        //Begin spawning the enemy.
        StartCoroutine(InitializeEnemy());
    }

    private void Update()
    {
        //Check if the enemy is inizialized.
        if (!isInitialized || isDead) return;

        //Check if the enemy is in attack range.
        if(isInAttackRange && !player.isDead)
        {
            //Rotate the enemy towards the player.
            Vector3 dirToPlayer = (target.position - transform.position).normalized;
            Quaternion rotToPlayer = Quaternion.LookRotation(new Vector3(dirToPlayer.x, 0, dirToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, Time.deltaTime / 0.07f);

            //Check if the enemy can attack.
            if (Time.time > attackTimer) StartCoroutine(Slash());
        }
    }

    public void ApplyDamage(int damage, Vector3 hitPoint)
    {
        //Enemy is already dead.
        if (health <= 0) return;

        //Apply damage on the enemy.
        health -= damage;

        //Check if the enemy died.
        if (health <= 0)
        {
            //Stop the active coroutines and then start destroying the enemy.
            StopAllCoroutines();
            StartCoroutine(DestroyEnemy(hitPoint));
        }
    }

    private IEnumerator Slash()
    {
        //Play the attacking animation and update the attack timer.
        animator.SetTrigger("Slash");
        attackTimer = Time.time + 1.2f;

        //Sync the damage to the animation.
        yield return new WaitForSeconds(0.5f);

        //Check if the enemy is still alive to finish the slashing attack.
        if (!isDead)
        {
            //Calculate the distance and angle to the target.
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            float angleToTarget = Vector3.Angle(transform.forward, (target.position - transform.position).normalized);

            //Apply damage on the player if in range and view.
            if (distanceToTarget < slashDistance && angleToTarget < 60f) PlayerManager.Instance.ApplyDamage(1, transform.position);
        }
    }

    private IEnumerator InitializeEnemy()
    {
        //Disable the enemy model and hitbox.
        GetComponent<CapsuleCollider>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);

        //Wait for the spawning to finish.
        yield return new WaitForSeconds(1.5f);

        //Enable the enemy model and hitbox.
        GetComponent<CapsuleCollider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);

        //Update the initialized state.
        isInitialized = true;

        //Set the enemy's destination.
        StartCoroutine(UpdateDestination());
    }

    private IEnumerator UpdateDestination()
    {
        while(!isDead)
        {
            //Calculate and visualize the target position.
            Vector3 targetPosition = AI_Utilities.GetTargetNextFramePosition(transform.position, target.position, motor.data.controller.velocity);
            Debug.DrawLine(transform.position, targetPosition, Color.red, 0.15f);

            //Calculate a path to the player's next frame position.
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);

            //Update the destination.
            if (path.status == NavMeshPathStatus.PathComplete) agent.SetPath(path);
            else agent.SetDestination(target.position);

            //Calculate the distance to the target and check if the enemy is in attack range.
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            isInAttackRange = (distanceToTarget < attackDistance);
            agent.updateRotation = !isInAttackRange;

            //Match the animation speed to the movement.
            float moveMagnitude = new Vector2(agent.velocity.x, agent.velocity.z).magnitude / agent.speed;
            animator.SetFloat("MoveMagnitude", Mathf.Clamp01(moveMagnitude));

            //Delay the next update.
            yield return new WaitForSeconds(0.15f);
        }
    }

    private IEnumerator DestroyEnemy(Vector3 hitPoint)
    {
        //Set the dying state, and prepare the ragdoll.
        isDead = true;
        GetComponent<Collider>().enabled = false;
        animator.enabled = false;
        agent.enabled = false;

        //Enable physincs on the ragdoll parts.
        foreach (Rigidbody body in ragdollParts) body.isKinematic = false;

        //Apply an impulse on the ragdoll and unparent the sword.
        ragdollParts[0].AddExplosionForce(240, hitPoint, 1, 0.3f, ForceMode.Impulse);
        ragdollParts[ragdollParts.Length - 1].transform.parent = null;

        //Spawn the destroy effect at the enemy's position and update the wave manager.
        Instantiate(deathParticle, transform.position, transform.rotation);
        AI_WaveManager.Instance.killsInCurrentWave++;

        //Wait for the frame to end.
        yield return null;
    }
}
