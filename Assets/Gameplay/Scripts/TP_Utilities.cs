using UnityEngine;

public static class TP_Utilities
{
    public static Vector3 GetTargetMoveVelocity(Vector2 inputDir, float moveSpeed)
    {
        Vector3 velocity = new Vector3(inputDir.x * moveSpeed, 0f, inputDir.y * moveSpeed);
        return velocity;
    }
}

public struct CharacterStates
{
    public bool isMoving;
    public bool isDashing;
}

public struct CharacterData
{
    public CharacterController controller;
    public Vector3 mousePoint;
    public Vector3 moveVelocity;
}
