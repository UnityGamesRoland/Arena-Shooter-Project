using UnityEngine;

public class TP_Animations : MonoBehaviour
{
    public Animator animator;
    public Transform IKRightHandSampler;
    public Transform IKLeftHandSampler;

    public bool isIKActive;
    public bool isAiming;

    private float angularVelocity;
    private float currentIKWeight;
    private float currentMeleeAttackProgress;

    private Vector3 currentLeftHandRotationVector;
    private Vector2 currentMoveVelocity;

    private CharacterIK IKTargeting;

    #region Singleton
    public static TP_Animations Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        IKTargeting = new CharacterIK();
    }
    #endregion

    public void AnimateCharacter(Vector2 inputDir, float moveMagnitude)
    {
        bool moving = TP_Motor.Instance.states.isMoving;

        //MOVEMENT
        animator.SetBool("Moving", moving);

        //ANGULAR VELOCITY
        float targetAngularVelocity = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).normalized.magnitude;
        angularVelocity = Mathf.Lerp(angularVelocity, targetAngularVelocity, Time.deltaTime / 0.2f);
        animator.SetFloat("AngularVelocity", Mathf.Clamp(angularVelocity, 0f, 0.35f));

        //PLAYBACK SPEED
        float magnitude = moving ? Mathf.Clamp01(moveMagnitude) : 1f;
        animator.SetFloat("PlaybackSpeed", magnitude);

        //LOCAL MOVE DIRECTION
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.up, new Vector3(1, 0, 1)).normalized;
        Vector3 move = inputDir.y * cameraForward + inputDir.x * Camera.main.transform.right;
        move.Normalize();
        Vector3 localMove = transform.InverseTransformDirection(move);

        //Turn, Forward
        currentMoveVelocity = new Vector2(Mathf.Clamp(localMove.x * moveMagnitude, -1, 1), Mathf.Clamp(localMove.z * moveMagnitude, -1, 1));

        animator.SetFloat("Forward", currentMoveVelocity.y, 0.05f, Time.deltaTime);
        animator.SetFloat("Turn", currentMoveVelocity.x, 0.05f, Time.deltaTime);
    }

    public void EnterAimState()
    {
        isAiming = true;

        Vector3 targetIKLeftHandOffset = IKTargeting.GetLeftHandOffset(currentMoveVelocity.x, currentMoveVelocity.y);
        Vector3 targetIKLeftHandPosition = Vector3.Lerp(IKLeftHandSampler.position - transform.position, transform.forward * targetIKLeftHandOffset.z + transform.right * targetIKLeftHandOffset.x + Vector3.up * targetIKLeftHandOffset.y, 0.6f);

        IKTargeting.currentLeftHandPosition = targetIKLeftHandPosition;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //RIGHT HAND TARGETING
        Vector3 targetIKRightHandOffset = IKTargeting.GetRightHandOffset(currentMoveVelocity.x, currentMoveVelocity.y);
        Vector3 targetIKRightHandPosition = Vector3.Lerp(IKRightHandSampler.position - transform.position, transform.forward * targetIKRightHandOffset.z + transform.right * targetIKRightHandOffset.x + Vector3.up * targetIKRightHandOffset.y, 0.45f);

        IKTargeting.currentRightHandPosition = Vector3.Lerp(IKTargeting.currentRightHandPosition, targetIKRightHandPosition, Time.deltaTime * 10);
        IKTargeting.currentRightHandRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(65f, 0f, -90f));

        if (isAiming)
        {
            //IN COMBAT LEFT HAND TARGETING
            Vector3 targetIKLeftHandOffset = IKTargeting.GetLeftHandOffset(currentMoveVelocity.x, currentMoveVelocity.y);
            Vector3 targetIKLeftHandPosition = Vector3.Lerp(IKLeftHandSampler.position - transform.position, transform.forward * targetIKLeftHandOffset.z + transform.right * targetIKLeftHandOffset.x + Vector3.up * targetIKLeftHandOffset.y, 0.6f);

            IKTargeting.currentLeftHandPosition = Vector3.Lerp(IKTargeting.currentLeftHandPosition, targetIKLeftHandPosition, Time.deltaTime * 10);
            IKTargeting.currentLeftHandRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 0f, 90f));
        }

        else
        {
            //OUT OF COMBAT LEFT HAND TARGETING
            Vector3 targetIKLeftHandOffset = IKTargeting.leftHandPoints[9];
            Vector3 targetIKLeftHandPosition = Vector3.Lerp(IKLeftHandSampler.position - transform.position, transform.forward * targetIKLeftHandOffset.z + transform.right * targetIKLeftHandOffset.x + Vector3.up * targetIKLeftHandOffset.y, 0.6f);

            Quaternion targetLeftHandRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(55f, 0f, 90f));

            IKTargeting.currentLeftHandPosition = Vector3.Lerp(IKTargeting.currentLeftHandPosition, targetIKLeftHandPosition, Time.deltaTime * 10);
            IKTargeting.currentLeftHandRotation = Quaternion.Slerp(IKTargeting.currentLeftHandRotation, targetLeftHandRotation, Time.deltaTime * 10);
        }

        currentIKWeight = Mathf.Lerp(currentIKWeight, isIKActive ? 1 : 0, Time.deltaTime * (isIKActive ? 2.5f : 20f));

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, currentIKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, currentIKWeight);
        animator.SetIKPosition(AvatarIKGoal.RightHand, transform.position + IKTargeting.currentRightHandPosition);
        animator.SetIKRotation(AvatarIKGoal.RightHand, IKTargeting.currentRightHandRotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, currentIKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, currentIKWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, transform.position + IKTargeting.currentLeftHandPosition);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, IKTargeting.currentLeftHandRotation);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(IKRightHandSampler.position, 0.05f);
            Gizmos.DrawWireSphere(IKLeftHandSampler.position, 0.05f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + IKTargeting.GetRightHandOffset(currentMoveVelocity.x, currentMoveVelocity.y), 0.05f);
            Gizmos.DrawWireSphere(transform.position + (isAiming ? IKTargeting.GetLeftHandOffset(currentMoveVelocity.x, currentMoveVelocity.y) : IKTargeting.leftHandPoints[9]), 0.05f);
        }
    }

    private class CharacterIK
    {
        public Vector3[] rightHandPoints;
        public Vector3[] leftHandPoints;

        public Vector3 currentRightHandPosition;
        public Vector3 currentLeftHandPosition;

        public Quaternion currentRightHandRotation;
        public Quaternion currentLeftHandRotation;

        public CharacterIK()
        {
            //RIGHT HAND IK POSITIONS
            rightHandPoints = new Vector3[9];

            rightHandPoints[0] = new Vector3(0.55f, 0.6f, -0.05f);   //Idle
            rightHandPoints[1] = new Vector3(0.5f, 0.6f, -0.05f);  //Forward
            rightHandPoints[2] = new Vector3(0.5f, 0.6f, -0.05f);  //Backward
            rightHandPoints[3] = new Vector3(0.4f, 0.6f, -0.05f);   //Left
            rightHandPoints[4] = new Vector3(0.4f, 0.6f, -0.05f);   //Right

            rightHandPoints[5] = new Vector3(0.55f, 0.6f, -0.05f);   //Forward Left
            rightHandPoints[6] = new Vector3(0.4f, 0.6f, -0.05f);   //Forward Right
            rightHandPoints[7] = new Vector3(0.5f, 0.6f, -0.05f);   //Backward Left
            rightHandPoints[8] = new Vector3(0.5f, 0.6f, -0.05f);   //Backward Right

            //LEFT HAND IK POSITIONS
            leftHandPoints = new Vector3[10];

            leftHandPoints[0] = new Vector3(-0.15f, 1.3f, 1f);   //Idle
            leftHandPoints[1] = new Vector3(-0.05f, 1.2f, 1f);  //Forward
            leftHandPoints[2] = new Vector3(-0.05f, 1.25f, 1f);  //Backward
            leftHandPoints[3] = new Vector3(0f, 1.25f, 1f);      //Left
            leftHandPoints[4] = new Vector3(0.05f, 1.25f, 1f);   //Right

            leftHandPoints[5] = new Vector3(-0.12f, 1.25f, 1f);   //Forward Left
            leftHandPoints[6] = new Vector3(-0.03f, 1.25f, 1f);   //Forward Right
            leftHandPoints[7] = new Vector3(-0.05f, 1.25f, 1f);   //Backward Left
            leftHandPoints[8] = new Vector3(-0.15f, 1.25f, 1f);   //Backward Right
            leftHandPoints[9] = new Vector3(-0.33f, 0.62f, 0.13f);   //Out of Combat

            //INITIAL POINT
            currentRightHandPosition = rightHandPoints[0];
            currentLeftHandPosition = leftHandPoints[0];
        }

        public Vector3 GetRightHandOffset(float velocityX, float velocityY)
        {
            if (velocityY > 0)
            {
                if (velocityX > 0) return Vector3.Lerp(rightHandPoints[0], rightHandPoints[6], Mathf.Abs(velocityX));

                if (velocityX < 0) return Vector3.Lerp(rightHandPoints[0], rightHandPoints[5], Mathf.Abs(velocityX));

                else return Vector3.Lerp(rightHandPoints[0], rightHandPoints[1], Mathf.Abs(velocityY));
            }

            else if (velocityY < 0)
            {
                if (velocityX > 0) return Vector3.Lerp(rightHandPoints[0], rightHandPoints[8], Mathf.Abs(velocityX));

                if (velocityX < 0) return Vector3.Lerp(rightHandPoints[0], rightHandPoints[7], Mathf.Abs(velocityX));

                else return Vector3.Lerp(rightHandPoints[0], rightHandPoints[2], Mathf.Abs(velocityY));
            }

            else
            {
                if (velocityX > 0) return Vector3.Lerp(rightHandPoints[0], rightHandPoints[4], Mathf.Abs(velocityX));

                if (velocityX < 0) return Vector3.Lerp(rightHandPoints[0], rightHandPoints[3], Mathf.Abs(velocityX));

                else return rightHandPoints[0];
            }
        }

        public Vector3 GetLeftHandOffset(float velocityX, float velocityY)
        {
            if (velocityY > 0)
            {
                if (velocityX > 0) return Vector3.Lerp(leftHandPoints[0], leftHandPoints[6], Mathf.Abs(velocityX));

                if (velocityX < 0) return Vector3.Lerp(leftHandPoints[0], leftHandPoints[5], Mathf.Abs(velocityX));

                else return Vector3.Lerp(leftHandPoints[0], leftHandPoints[1], Mathf.Abs(velocityY));
            }

            else if (velocityY < 0)
            {
                if (velocityX > 0) return Vector3.Lerp(leftHandPoints[0], leftHandPoints[8], Mathf.Abs(velocityX));

                if (velocityX < 0) return Vector3.Lerp(leftHandPoints[0], leftHandPoints[7], Mathf.Abs(velocityX));

                else return Vector3.Lerp(leftHandPoints[0], leftHandPoints[2], Mathf.Abs(velocityY));
            }

            else
            {
                if (velocityX > 0) return Vector3.Lerp(leftHandPoints[0], leftHandPoints[4], Mathf.Abs(velocityX));

                if (velocityX < 0) return Vector3.Lerp(leftHandPoints[0], leftHandPoints[3], Mathf.Abs(velocityX));

                else return leftHandPoints[0];
            }
        }
    }
}
