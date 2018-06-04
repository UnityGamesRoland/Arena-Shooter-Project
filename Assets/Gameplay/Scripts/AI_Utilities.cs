using UnityEngine;

public static class AI_Utilities
{
    public static Vector3 GetTargetNextFramePosition(Vector3 enemyPosition, Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 nextFramePosition = targetPosition + targetVelocity * 0.2f + ((targetPosition - enemyPosition).normalized * 0.25f) * targetVelocity.normalized.magnitude;
        return nextFramePosition;
    }
}
