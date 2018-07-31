using UnityEngine;

public class Ability_Orb : MonoBehaviour
{
    private Transform target;
    private bool isAttacking = false;

    private void Start()
    {
        target = TP_Motor.Instance.transform;
    }

    private void Update()
    {
        if(!isAttacking)
        {
            transform.RotateAround(transform.position, Vector3.up, 100 * Time.deltaTime);
            transform.position = target.position - transform.forward * 1.3f;
        }
    }
}
