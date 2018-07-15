using UnityEngine;

public static class TP_Utilities
{
    public static Vector3 GetTargetMoveVelocity(Vector2 inputDir, float moveSpeed)
    {
        Vector3 velocity = new Vector3(inputDir.x * moveSpeed, 0f, inputDir.y * moveSpeed);
        return velocity;
    }

    public static int GetAxis(string axis, KeybindingsProfile keybindings)
    {
        if(axis == "Horizontal")
        {
            if (Input.GetKey(keybindings.moveRightKey)) return 1;
            else if (Input.GetKey(keybindings.moveLeftKey)) return -1;
            else return 0;
        }

        if (axis == "Vertical")
        {
            if (Input.GetKey(keybindings.moveForwardKey)) return 1;
            else if (Input.GetKey(keybindings.moveBackwardKey)) return -1;
            else return 0;
        }

        else return 0;
    }

    public static bool GetAction(string action, KeybindingsProfile keybindings)
    {
        if(action == "Shoot")
        {
            if (Input.GetKey(keybindings.shootKey)) return true;
            else return false;
        }

        if (action == "Dash")
        {
            if (Input.GetKeyDown(keybindings.dashKey)) return true;
            else return false;
        }

        if (action == "Dash")
        {
            if (Input.GetKeyDown(keybindings.dashKey)) return true;
            else return false;
        }

        if (action == "ActivateShield")
        {
            if (Input.GetKeyDown(keybindings.shieldKey)) return true;
            else return false;
        }

        if (action == "DeactivateShield")
        {
            if (Input.GetKeyUp(keybindings.shieldKey)) return true;
            else return false;
        }

        else return false;
    }
}

public struct CharacterStates
{
    public bool isMoving;
    public bool isDashing;
    public bool enumeratingDash;
}

public struct CharacterData
{
    public CharacterController controller;
    public Vector3 mousePoint;
    public Vector3 moveVelocity;
    public Vector2 inputDirection;
}
