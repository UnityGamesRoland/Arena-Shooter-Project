using UnityEngine;
using System.Collections;

public class TP_Camera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private Vector3 actualOffset;
    private Vector3 moveInterpolationVelocity;

    private void Start()
    {
        StartCoroutine(RaiseCamera());
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + actualOffset, ref moveInterpolationVelocity, 0.07f);
    }

    private IEnumerator RaiseCamera()
    {
        actualOffset = new Vector3(offset.x, offset.y - 1.3f, offset.z);

        while(actualOffset.y < offset.y)
        {
            actualOffset.y = Mathf.MoveTowards(actualOffset.y, offset.y + 0.1f, Time.unscaledDeltaTime * 0.8f);
            yield return null;
        }
    }
}
