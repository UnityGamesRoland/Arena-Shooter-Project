using UnityEngine;

public class TP_Animations : MonoBehaviour
{
    public Animator animator;

    #region Singleton
    public static TP_Animations Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    #endregion

    public void AnimateCharacter(Vector2 inputDir, float moveMagnitude)
    {
        //Get the different states for the animator.
        bool moving = TP_Motor.Instance.states.isMoving;

        //Set the animator's paramaters.
        animator.SetBool("Moving", moving);

        //Calculate the move magnitude and set the animator's paramater.
        float magnitude = moving ? Mathf.Clamp01(moveMagnitude) : 1f;
        animator.SetFloat("PlaybackSpeed", magnitude);

        //Get the relative move direction.
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.up, new Vector3(1, 0, 1)).normalized;
        Vector3 move = inputDir.y * cameraForward + inputDir.x * Camera.main.transform.right;

        //Normalize the move direction.
        move.Normalize();

        //Transform the move direction to local space.
        Vector3 localMove = transform.InverseTransformDirection(move);

        //Clamp the animation blend amount.
        float forward = Mathf.Clamp(localMove.z * moveMagnitude, -1, 1);
        float turn = Mathf.Clamp(localMove.x * moveMagnitude, -1, 1);

        //Set the animator paramaters.
        animator.SetFloat("Forward", forward, 0.05f, Time.deltaTime);
        animator.SetFloat("Turn", turn, 0.05f, Time.deltaTime);
    }
}
