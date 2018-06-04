using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class DynamicObstacle : MonoBehaviour
{
    public Transform frontWall;
    public Transform backWall;
    public Transform leftWall;
    public Transform rightWall;
    public Vector4 obstacleLength = Vector4.zero;

    public bool isMovingObstacle = true;
    public bool isRotatingObstacle = true;

    [ShowIf("isRotatingObstacle")] public float rotationSpeed = 15;
    [ShowIf("isMovingObstacle")] public float moveSpeed = 2;
    [ShowIf("isMovingObstacle")] public int initialTargetWaypointIndex;
    [ShowIf("isMovingObstacle")] public Transform[] waypoints;

    private void Start()
    {
        //Move the obstacle.
        if(isMovingObstacle) StartCoroutine(MoveObstacle());
    }

    private void Update()
    {
        //Update the obstacle's rotation and scale.
        UpdateObstacleLengths();
        if(isRotatingObstacle) SpinObstacle();
    }

    private void UpdateObstacleLengths()
    {
        //Update the scale of the walls.
        frontWall.localScale = new Vector3(0.7f, obstacleLength.z, 1f);
        backWall.localScale = new Vector3(0.7f, obstacleLength.w, 1f);
        leftWall.localScale = new Vector3(0.7f, obstacleLength.x, 1f);
        rightWall.localScale = new Vector3(0.7f, obstacleLength.y, 1f);

        //Update the visibility of the walls.
        frontWall.gameObject.SetActive(frontWall.localScale.y < 0.2f ? false : true);
        backWall.gameObject.SetActive(backWall.localScale.y < 0.2f ? false : true);
        leftWall.gameObject.SetActive(leftWall.localScale.y < 0.2f ? false : true);
        rightWall.gameObject.SetActive(rightWall.localScale.y < 0.2f ? false : true);
    }

    private void SpinObstacle()
    {
        //Rotate the obstacle.
        if(rotationSpeed != 0) transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
    }

    private IEnumerator MoveObstacle()
    {
        //Get the target waypoint.
        int targetWaypointIndex = initialTargetWaypointIndex;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex].position;

        //Wait for the level initialization. Happening in the AI_WaveManager.
        yield return new WaitForSeconds(1);

        while(true)
        {
            //Move the obstacle.
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, Time.deltaTime * moveSpeed);

            //Check if the obstacle has reached the target position.
            if(transform.position == targetWaypoint)
            {
                //Get the next waypoint.
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex].position;
            }

            //Wait for the frame to end.
            yield return null;
        }
    }
}
