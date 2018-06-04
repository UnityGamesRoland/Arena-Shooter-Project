using UnityEngine;

public class TP_Camera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private Vector3 moveInterpolationVelocity;

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref moveInterpolationVelocity, 0.07f);
    }
}
