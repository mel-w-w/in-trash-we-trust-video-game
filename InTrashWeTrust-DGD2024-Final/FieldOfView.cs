using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius = 4f;
    public float outerRadius = 6f;
    [Range(1, 360)] public float angle;
    [Range(1, 360)] public float rangeAngle = 45f;

    public LayerMask targetLayer;
    public LayerMask obstructionLayer;

    public GameObject playerRef;

    public PlayerMovement player;

    // public bool CanSeePlayer { get; private set; }
    // public bool CanSensePlayer { get; private set; }
    public bool CanSeePlayer = false;
    public bool CanSensePlayer = false;

    Rigidbody2D rb;

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(FOVCheck());
        
    }

    private IEnumerator FOVCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(0.0f);

        while (true)
        {
            yield return wait;
            FOV();
        }
    }

    private void FOV()
    {
        if (Vector2.Distance(transform.position, playerRef.transform.position) <= outerRadius && !player.hiding && !player.isEating)
        {
            CanSensePlayer = true;
            // RotateTowardsPlayer();

        }else{
            CanSensePlayer = false;
        }
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        if (rangeCheck.Length > 0)
        {
            foreach (Collider2D collider in rangeCheck)
            {
                Transform target = collider.transform;
                Vector2 directionToTarget = (target.position - transform.position).normalized;

                // Calculate the angle between the forward direction and the direction to the target
                float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);

                // Calculate the difference between the angle to the target and the FOV angle
                float angleDifference = Mathf.Abs(angleToTarget - angle);
                //Ensure right side has same functionality
                if (angleDifference > 180){
                    angleDifference = Mathf.Abs(360 - angleDifference);
                }

                // Ensure the angle difference is within half of the range angle
                if (angleDifference < (rangeAngle / 2) && !player.hiding && !player.isEating)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, target.position);
                    if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                    {
                        CanSeePlayer = true;
                        // Draw the raycast line
                        Debug.DrawRay(transform.position, directionToTarget * distanceToTarget, Color.red);
                        break; // Exit the loop early since we found a visible target
                    }
                }
            }
        }
        else if (CanSeePlayer)
        {
            CanSeePlayer = false;
        }
        
    }

    // private void RotateTowardsPlayer()
    // {
    //     Vector2 directionToPlayer = (playerRef.transform.position - transform.position).normalized;
    //     float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

    //     // Calculate the rotation angle based on the direction to the player
    //     float desiredAngle = angleToPlayer - 90f; // Adjust as needed based on the FOV object's initial orientation

    //     // Smoothly rotate towards the player
    //     Quaternion targetRotation = Quaternion.Euler(0f, 0f, desiredAngle);
    //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    // }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, outerRadius); // Outer circle


        Vector3 forwardDirection = transform.up; // Use transform.up instead of Quaternion.Euler(0, 0, transform.eulerAngles.z) * Vector3.up

        // Calculate the angles for left and right boundaries
        float leftAngle = angle + (rangeAngle / 2);
        float rightAngle = angle - (rangeAngle / 2);

        // Calculate the directions for left and right boundaries
        Vector3 leftBoundary = Quaternion.Euler(0, 0, leftAngle) * Vector3.up * radius;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, rightAngle) * Vector3.up * radius;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        if (CanSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, playerRef.transform.position);
        }
    }
}
