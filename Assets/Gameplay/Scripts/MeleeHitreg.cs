using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeHitreg : MonoBehaviour
{
    public Transform[] startPoints;
    public LayerMask hitMask;

    public void EnableHitReg(float duration)
    {
        StopCoroutine(RegisterHits(0));
        StartCoroutine(RegisterHits(duration));
    }

    public void DisableHitreg()
    {
        StopCoroutine(RegisterHits(0));
    }

    private IEnumerator RegisterHits(float duration)
    {
        List<GameObject> objectsHit = new List<GameObject>();
        Vector3[] previousFramePoints = new Vector3[startPoints.Length];

        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            for (int i = 0; i < startPoints.Length; i++) previousFramePoints[i] = startPoints[i].position;

            yield return null;

            for (int i = 0; i < startPoints.Length; i++)
            {
                RaycastHit hit;

                Debug.DrawLine(startPoints[i].position, previousFramePoints[i], Color.red, 0.1f);

                if (Physics.Linecast(startPoints[i].position, previousFramePoints[i], out hit, hitMask, QueryTriggerInteraction.Collide))
                {
                    GameObject hitObject = hit.transform.root.gameObject;

                    if (!objectsHit.Contains(hitObject))
                    {
                        objectsHit.Add(hitObject);
                        Debug.Log("Hit " + hitObject.name);
                    }
                }
            }
        }
    }
}
