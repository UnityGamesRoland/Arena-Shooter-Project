using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class AI_Shooter : MonoBehaviour
{
    public WeaponBullet bulletPrefab;
    public Transform muzzleTransform;
    public Transform deathParticle;
    public AudioClip shootSound;
    public LayerMask arenaEdgeLayer;
    public LayerMask arenaCoverLayer;
    public LayerMask playerBubbleLayer;

    private float inLineOfSightTimer = 0f;
    private float outOfSightTimer = 0f;
    private float spread = 0.8f;
    private float weaponChargeTime = 1f;
    private int health = 3;

    private bool isInitialized;
    private bool isRagdoll;
    private bool isDead;
    private bool isInLineOfSight;

    private Rigidbody[] ragdollParts;
    private NavMeshAgent agent;
    private Transform target;
    private ParticleSystem muzzleFlash;
    private AudioSource source;

    private Animator animator;
    private TP_Motor motor;

    private void Awake()
    {
        //References.
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        muzzleFlash = muzzleTransform.GetComponent<ParticleSystem>();
        source = muzzleTransform.GetComponent<AudioSource>();

        //Get the ragdoll parts and make them kinematic.
        ragdollParts = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in ragdollParts) body.isKinematic = true;
    }

    private void Start()
    {
        //Initalization.
        motor = TP_Motor.Instance;
        target = motor.transform;

        //Begin spawning the enemy.
        StartCoroutine(InitializeEnemy());
    }

    private void Update()
    {
        if(isInitialized)
        {
            //Match the animation speed to the movement.
            Vector2 moveDir = new Vector2(agent.velocity.x, agent.velocity.z).normalized;
            AnimateCharacter(moveDir, moveDir.magnitude);
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

    private void AnimateCharacter(Vector2 moveDir, float moveMagnitude)
    {
        //Set the animator's paramaters.
        animator.SetBool("Moving", moveMagnitude > 0);

        //Get the relative move direction.
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.up, new Vector3(1, 0, 1)).normalized;
        Vector3 move = moveDir.y * cameraForward + moveDir.x * Camera.main.transform.right;

        //Normalize the move direction.
        move.Normalize();

        //Transform the move direction to local space.
        Vector3 localMove = transform.InverseTransformDirection(move);

        //Clamp the animation blend amount.
        float forward = Mathf.Clamp(localMove.z * moveMagnitude, -1, 1);
        float turn = Mathf.Clamp(localMove.x * moveMagnitude, -1, 1);

        //Set the animator paramaters.
        animator.SetFloat("Forward", forward, 0.05f, Time.deltaTime);
        animator.SetFloat("Turn", turn, 0.05f, Time.deltaTime);
    }

    private void MoveToNewPoint(bool requireLineOfSight)
    {
        //Calculate and visualize the target position.
        Vector3 targetPosition = GetEnemyNextDestination(requireLineOfSight);

        //Calculate a path to the player's next frame position.
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);

        //Update the destination.
        if (path.status == NavMeshPathStatus.PathComplete) agent.SetPath(path);
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
        StartCoroutine(UpdateRotation());
        StartCoroutine(UpdateLineOfSight());
        StartCoroutine(HandleShooting());
    }

    private IEnumerator UpdateRotation()
    {
        agent.updateRotation = false;

        while(!isDead)
        {
            Quaternion targetRotation = AI_Utilities.GetRotationToTarget(transform.position, target.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 360);
            yield return null;
        }
    }

    private IEnumerator UpdateDestination()
    {
        while (!isDead)
        {
            if ((target.position - transform.position).sqrMagnitude < 7f && inLineOfSightTimer > 2f)
            {
                MoveToNewPoint(false);
            }

            if (agent.remainingDistance <= 0.5f)
            {
                yield return new WaitForSeconds(0.5f);

                if (outOfSightTimer > 2)
                {
                    MoveToNewPoint(true);
                    outOfSightTimer = 0f;
                }

               else MoveToNewPoint(false);
            }

            //Delay the next update.
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator UpdateLineOfSight()
    {
        while(!isDead)
        {
            isInLineOfSight = !Physics.Linecast(transform.position + Vector3.up * 1.4f, target.position + Vector3.up * 0.4f, arenaCoverLayer, QueryTriggerInteraction.Collide);

            if (isInLineOfSight)
            {
                outOfSightTimer = 0f;
                inLineOfSightTimer += 0.25f;

                yield return new WaitForSeconds(0.25f);
            }

            else if(!isInLineOfSight)
            {
                inLineOfSightTimer = 0f;
                outOfSightTimer += 0.1f;

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private IEnumerator HandleShooting()
    {
        float weaponCharge = 0f;
        float shootTimer = 0f;

        //Initial wait time after spawn.
        yield return new WaitForSeconds(1f);

        while(!isDead)
        {
            if (isInLineOfSight)
            {
                if (Time.time > shootTimer)
                {
                    if (!muzzleFlash.isPlaying) muzzleFlash.Play();
                    weaponCharge += 0.1f;

                    if(weaponCharge >= weaponChargeTime)
                    {
                        Shoot();
                        shootTimer = Time.time + 1.5f;
                    }
                }

                else weaponCharge = 0f;
            }

            else
            {
                if (muzzleFlash.isPlaying) muzzleFlash.Stop();
                if (weaponCharge > 0.1f) weaponCharge -= 0.1f;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Shoot()
    {
        //Calculate the spread amount.
        Vector2 randomizedSpread = Random.insideUnitCircle * spread;

        //Instantiate the bullet at the muzzle and apply the spread on it's rotation.
        Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.Euler(transform.eulerAngles + new Vector3(randomizedSpread.x, randomizedSpread.y, 0)));

        //Play the shoot sound.
        source.pitch = Random.Range(0.97f, 1f);
        source.PlayOneShot(shootSound);

        //Stop the muzzle flash particle.
        muzzleFlash.Stop();
        muzzleFlash.Clear();
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
        //AI_WaveManager.Instance.killsInCurrentWave++;

        //Stop the muzzle flash particle.
        muzzleFlash.Stop();
        muzzleFlash.Clear();

        //Wait for the frame to end.
        yield return null;
    }

    private Vector3 GetEnemyNextDestination(bool requireLineOfSight)
    {
        List<Vector3> possibleMovePoints = new List<Vector3>();
        Vector3 backupMovePoint = Vector3.zero;

        float backupPointsDistanceFromPlayer = 100f;
        float minDistanceFromEdge = 5f;
        float minDistanceFromPlayer = 5f;

        float rayCount = 30f;
        float angle = 0f;

        for (int i = 0; i < rayCount; i++)
        {
            float distanceDeviationPercent = Random.Range(0.05f, 0.6f);

            float posX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float posZ = Mathf.Sin(angle * Mathf.Deg2Rad);
            angle += 360 / rayCount;

            Vector3 dir = transform.right * posX + transform.forward * posZ;

            Ray rayToArenaEdge = new Ray(transform.position, dir);
            RaycastHit hitToArenaEdge;

            //POINT AT ARENA EDGE
            if (Physics.Raycast(rayToArenaEdge, out hitToArenaEdge, 100, arenaEdgeLayer, QueryTriggerInteraction.Collide))
            {
                float distanceFromEdgeToAI = Vector3.Distance(hitToArenaEdge.point, transform.position);

                //POINT WITH OPTIMAL DISTANCE FROM EDGE
                if (distanceFromEdgeToAI >= minDistanceFromEdge)
                {
                    float maxDistanceDeviation = Vector3.Distance(hitToArenaEdge.point, transform.position);
                    Vector3 possibleMovePoint = hitToArenaEdge.point - dir * (maxDistanceDeviation * distanceDeviationPercent);
                    float distanceFromPlayer = Vector3.Distance(possibleMovePoint, target.position);

                    //POINT WITH OPTIMAL DISTANCE FROM PLAYER
                    if (distanceFromPlayer >= minDistanceFromPlayer)
                    {
                        //POINT WHICH'S PATH IS NOT GOING NEAR THE PLAYER
                        if(!Physics.Linecast(transform.position, possibleMovePoint, playerBubbleLayer, QueryTriggerInteraction.Collide))
                        {
                            //BACKUP POINT WHICH IS THE CLOSEST OPTIMAL POINT TO THE PLAYER
                            if (distanceFromPlayer < backupPointsDistanceFromPlayer)
                            {
                                backupMovePoint = possibleMovePoint;
                                backupPointsDistanceFromPlayer = distanceFromPlayer;
                            }

                            //POINT WHERE PLAYER DOESN'T HAVE TO BE IN SIGHT
                            if (!requireLineOfSight)
                            {
                                possibleMovePoints.Add(possibleMovePoint);
                            }

                            //POINT WHERE PLAYER HAS TO BE IN SIGHT
                            else
                            {
                                if (!Physics.Linecast(possibleMovePoint + Vector3.up * 1.4f, target.position + Vector3.up * 0.4f, arenaCoverLayer, QueryTriggerInteraction.Collide))
                                {
                                    possibleMovePoints.Add(possibleMovePoint);
                                }
                            }
                            
                        }
                    }
                }
            }

        }

        if (possibleMovePoints.Count > 0) return possibleMovePoints[Random.Range(0, possibleMovePoints.Count)];
        else return backupMovePoint;

    }
}
