using UnityEngine;

public class WeaponBullet : MonoBehaviour
{
	public GameObject surfaceHitEffect;
	public LayerMask collisionLayer;
    public int bulletHealth = 1;
    public int bulletDamage = 1;
    public float bulletRadius = 0.5f;
    public float bulletSpeed = 70f;

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
                hit.transform.GetComponent<AI_Runner>().ApplyDamage(bulletDamage, hit.point + hit.normal * 0.15f);

                //Destroy the bullet.
                bulletHealth--;
                if (bulletHealth == 0) Destroy(gameObject);
            }

            //On Hit: Other
            else
            {
                //Spawn hit effect.
                GameObject effect = Instantiate(surfaceHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(effect, 0.75f);

                //Destroy the bullet.
                Destroy(gameObject);
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
                //Apply damage on the current enemy and remove one health from the bullet.
                initialCollisions[i].GetComponent<AI_Runner>().ApplyDamage(bulletDamage, initialCollisions[i].transform.position);

                //Destroy the bullet.
                bulletHealth --;
				if(bulletHealth == 0) Destroy(gameObject);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, bulletRadius);
	}
}
