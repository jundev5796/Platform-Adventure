using UnityEngine;

public class Trap_FallingPlatform : MonoBehaviour
{
    [SerializeField] private float speed = 0.75f;
    [SerializeField] private float travelDistance;
    private Vector3[] wayPoints;
    private int wayPointIndex;
    public bool canMove;


    private void Start()
    {
        SetupWaypoints();
    }


    private void SetupWaypoints()
    {
        wayPoints = new Vector3[2];

        float yOffset = travelDistance / 2;

        wayPoints[0] = transform.position + new Vector3(0, yOffset, 0);
        wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0);
    }


    private void Update()
    {
        HandleMovement();
    }


    private void HandleMovement()
    {
        if (canMove == false)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayPointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayPointIndex]) < 0.1f)
        {
            wayPointIndex++;

            if (wayPointIndex >= wayPoints.Length)
                wayPointIndex = 0;
        }
    }
}