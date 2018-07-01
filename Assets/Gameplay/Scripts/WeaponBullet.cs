using UnityEngine;

public class WeaponBullet : MonoBehaviour
{
    public Transform trail;
	public GameObject surfaceHitEffect;
	public LayerMask collisionLayer;
    public int bulletHealth = 1;
    public int bulletDamage = 1;
    public float bulletRadius = 0.5f;
    public float bulletSpeed = 70f;
    public bool bulletOfEnemy = false;

	private void Start()
	{
		//In case the bullet doesn't hit anything, destroy it a few seconds after it spawned.
		Destroy(gameObject, 0.8f);

		//Check if the bullet spawns inside an enemy.
		CheckInitialHit();
	}

	private void Update()
	{
		//Get the amount of distance the bullet will move this frame.
		float moveDistance = bulletSpeed * Time.deltaTime;

		//Check if the bullet will hit something.
		CheckHit(moveDistance);

		//Move the bullet forward every frame.
		transform.Translate(Vector3.forward * moveDistance);
	}

	private void CheckHit(float moveDistance)
	{
		//Setup a forward ray.
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		//Check if the ray hits something.
		if(Physics.SphereCast(ray, bulletRadius, out hit, moveDistance, collisionLayer, QueryTriggerInteraction.Collide))
		{
            //On Hit: Enemy
            if (hit.transform.CompareTag("Enemy"))
            {
                //Apply damage and spawn hit effect.
                AI_Runner runner = hit.transform.GetComponent<AI_Runner>();
                AI_Shooter shooter = hit.transform.GetComponent<AI_Shooter>();

                if (runner != null) runner.ApplyDamage(bulletDamage, hit.point + hit.normal * 0.15f);
                if (shooter != null) shooter.ApplyDamage(bulletDamage, hit.point + hit.normal * 0.15f);

                if(!bulletOfEnemy)
                {
                    //Destroy the bullet.
                    bulletHealth--;
                    if (bulletHealth == 0) Destroy(gameObject);
                }
            }

            //On hit: Player
            else if (hit.transform.root.CompareTag("Player") && bulletOfEnemy)
            {
                //Apply damage and spawn hit effect.
                PlayerManager.Instance.ApplyDamage(1, transform.position);
            }

            //On Hit: Other
            else
            {
                //Spawn hit effect.
                GameObject effect = Instantiate(surfaceHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(effect, 0.75f);

                //Destroy the bullet.
                DestroyEnemyBullet();
            }
        }
	}

	private void CheckInitialHit()
	{
		//Get an array of collisions the bullet is intersecting with.
		Collider[] initialCollisions = Physics.OverlapSphere(transform.position, bulletRadius, collisionLayer, QueryTriggerInteraction.Collide);

		//Check the length of the array.
		if(initialCollisions.Length > 0)
		{
			//Loop through the initial hit array.
			for(int i = 0; i < initialCollisions.Length; i++)
			{
                if(!bulletOfEnemy)
                {
                    AI_Runner runner = initialCollisions[i].transform.GetComponent<AI_Runner>();
                    AI_Shooter shooter = initialCollisions[i].transform.GetComponent<AI_Shooter>();

                    if (runner != null) runner.ApplyDamage(bulletDamage, initialCollisions[i].transform.position);
                    if (shooter != null) shooter.ApplyDamage(bulletDamage, initialCollisions[i].transform.position);
                }

                else
                {
                    PlayerManager.Instance.ApplyDamage(1, transform.position);
                }

                //Destroy the bullet.
                bulletHealth --;
				if(bulletHealth == 0) Destroy(gameObject);
			}
		}
	}

    private void DestroyEnemyBullet()
    {
        trail.SetParent(null, true);
        Destroy(gameObject);
    }

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, bulletRadius);
	}
}
