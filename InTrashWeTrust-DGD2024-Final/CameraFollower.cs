using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;
    
    private Rigidbody2D targetRigidbody;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    void Start()
    {
        targetRigidbody = target.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        // Smoothly follow the target's position in both x and y directions
        Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z; // Retain the original z-coordinate of the camera
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);

    }
}
