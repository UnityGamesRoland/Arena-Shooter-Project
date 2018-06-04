using UnityEngine;
using Sirenix.OdinInspector;

public class CopyColliders : MonoBehaviour
{
    public Transform referenceObjectColliderHolder;

    [Button(ButtonSizes.Medium)]
    public void CopyCharacterColliders()
    {
        CapsuleCollider[] referenceColliders = referenceObjectColliderHolder.GetComponentsInChildren<CapsuleCollider>();
        CapsuleCollider[] targetColliders = transform.GetComponentsInChildren<CapsuleCollider>();

        for (int i = 0; i < referenceColliders.Length - 1; i++)
        {
            targetColliders[i].center = referenceColliders[i].center;
            targetColliders[i].radius = referenceColliders[i].radius;
            targetColliders[i].height = referenceColliders[i].height;
        }
    }
}
