using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private FieldOfView FOV;
    Rigidbody2D rb;
    Transform target;

    Vector2 moveDirection;

    private SpriteRenderer spriteRenderer;

    private Vector2 previousPosition;
    [SerializeField] float moveSpeed = 3f;
    public Animator animator;

    public float patrolRadius = 2f; // Radius of the circular patrol area
    private Vector3 patrolCenter; 
    private Vector3 patrolDestination;
    public PlayerMovement player;

    public AudioSource audioPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        FOV = GetComponent<FieldOfView>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousPosition = transform.position;
        audioPlayer.Play();
        patrolCenter = transform.position;
        SetPatrolDestination();
    }
        

    void FixedUpdate() {
        if (!player.hiding && !player.isEating) {
            // Execute AI logic only if the player is not hiding
            bool canSeePlayer = FOV.CanSeePlayer;
            bool canSensePlayer = FOV.CanSensePlayer;

            if (canSensePlayer && !canSeePlayer) {
                rb.velocity = Vector2.zero;
                animator.SetBool("OnGuard", true);
            } else {
                animator.SetBool("OnGuard", false);
            }

            if (canSensePlayer && canSeePlayer) {
                
                animator.SetBool("Seen", true);
                FOV.rangeAngle = 360f;
                target = GameObject.Find("LR").transform;
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                previousPosition = transform.position;
                

                // Flip the enemy sprite based on movement direction
                if (transform.position.x < target.position.x) {
                    spriteRenderer.flipX = true; // Facing left
                } else {
                    spriteRenderer.flipX = false; // Facing right
                }
            } else {
                FOV.rangeAngle = 45f;
                animator.SetBool("Seen", false);
                FOV.CanSeePlayer = false;
                FOV.CanSensePlayer = false;
                animator.SetBool("IdleWalk", true);
            }

            if (!canSeePlayer && !canSensePlayer) {
                animator.SetBool("IdleWalk", true);
                if (Vector3.Distance(transform.position, patrolDestination) > 0.1f) {
                    // Move towards the patrol destination
                    Vector3 moveDirection = (patrolDestination - transform.position).normalized;
                    rb.velocity = moveDirection * 1f;
                } else {
                    // Set a new patrol destination
                    SetPatrolDestination();
                }
            } else {
                animator.SetBool("IdleWalk", false);
            }
        } else {
            // patrolCenter = transform.position;
            animator.SetBool("IdleWalk", true); // Set animator to idle walk when player is hiding
            // FOV.angle = 0f; // Reset FOV angle when player is hiding
            // patrolCenter = transform.position;
            if (Vector3.Distance(transform.position, patrolDestination) > 0.1f) {
                // Move towards the patrol destination
                Vector3 moveDirection = (patrolDestination - transform.position).normalized;
                rb.velocity = moveDirection * 1f;
            }else{
                SetPatrolDestination();
            }
        }
    }


    private void SetPatrolDestination()
    {
        // Vector3 patrolDestination = tranform.position;
        // patrolCenter = tranform.position;
        float randomAngle = Random.Range(0f, Mathf.PI * 2f); // Random angle in radians
        float randomRadius = Random.Range(0f, patrolRadius); // Random radius within the patrol circle

        // Calculate the patrol destination based on polar coordinates
        float x = patrolCenter.x + randomRadius * Mathf.Cos(randomAngle);
        float y = patrolCenter.y + randomRadius * Mathf.Sin(randomAngle);

        // Set the new patrol destination
        patrolDestination = new Vector3(x, y, transform.position.z);
    }

    public void ResetDetectionFlags()
    {
        
        // Debug.Log("before: " + FOV.CanSeePlayer + FOV.CanSensePlayer);
        animator.SetBool("Seen", false);
        animator.SetBool("OnGuard", false);
        animator.SetBool("IdleWalk", true);
        FOV.CanSeePlayer = false;
        FOV.CanSensePlayer = false;
        // Debug.Log("after: " + FOV.CanSeePlayer + FOV.CanSensePlayer);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.CompareTag("Player")){
            spriteRenderer.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.CompareTag("Player")){
            spriteRenderer.enabled = true;
        }
    }
}
