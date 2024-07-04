using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour
{
    public Transform centerPoint; // Center point around which the pivot object rotates
    public float rotationSpeed = 120f;

    public Transform target;

    public FieldOfView FOV;
    

    void Start()
    {
        FOV = GetComponentInParent<FieldOfView>();
        // spriteRenderer = GetComponentInChild<SpriteRenderer>();
        
    }

    void Update()
    {
        bool canSeePlayer = FOV.CanSeePlayer;
        bool canSensePlayer = FOV.CanSensePlayer;
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        if (canSensePlayer && !canSeePlayer){
            Vector3 pivotDirection = transform.position;
            // Debug.Log("pivotDir: " + pivotDirection.x + pivotDirection.y + pivotDirection.z);
            float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);
            Vector3 rotationAxis = angleToTarget > 0 ? Vector3.forward : Vector3.back;
            transform.RotateAround(transform.position, rotationAxis, rotationSpeed * Time.deltaTime);
            // transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            float angle = transform.rotation.eulerAngles.z;

            // Calculate the angle using Vector3.up as the reference direction
            // float angle = Vector3.SignedAngle(Vector3.up, pivotDirection.normalized, Vector3.forward);
            FOV.angle = angle;
            // Debug.Log("pivot angle: " + angle);
        }
        if (canSeePlayer){
            
            float angle = Vector2.SignedAngle(transform.up, directionToTarget);
            // Debug.Log(angle);
            FOV.angle = angle;
            transform.up = directionToTarget;

        }
        
    }
}
