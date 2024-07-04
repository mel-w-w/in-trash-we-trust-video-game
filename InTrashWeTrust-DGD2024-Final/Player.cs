using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    private float speed;

    public bool isEating { get; private set; }
    public bool isFighting { get; private set; }
    public bool inTrash { get; private set; }

    public int foodCollected = 0;
    public int foodRequired = 5;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text timerText;
    public TMP_Text binWarningText;

    private int moveForce = 3;

    private float timeLimit = 110f;
    private float currentTime;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Transform playerTransform;

    public Animator animator;

    public bool hiding = false;
    private bool inBoundary = false;

    public AudioSource audioPlayer;

    // private GameSaveManager saveManager;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = transform;
        foodText.text = "Food: " + foodCollected + "/" + foodRequired;
        currentTime = timeLimit;
        timerText.text = "Time Left: " + Mathf.RoundToInt(timeLimit);
        isEating = false;
        isFighting = false;
        // binWarningText.text = "Not Enough Food Collected"; // Set the warning text
        binWarningText.gameObject.SetActive(false); 
        // saveManager = GameObject.Find("Manager").GetComponent<GameSaveManager>();
    }

    void Update()
    {
        ProcessInputs();
        FlipPlayer();

        // decrease the timer every frame
        currentTime -= Time.deltaTime;
        timerText.text = "Time Left: " + Mathf.RoundToInt(currentTime);

        // Game Over
        if (currentTime <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            ; // TODO: player death, game over
        }

        
        // Debug.Log(PlayerPrefs.GetInt("MiniGameCompleted"));
        // if(PlayerPrefs.GetInt("MiniGameCompleted") == 1) {
        //     foodCollected++;
        //     saveManager.ReturnFromMiniGameScene();
        //     Debug.Log("Minigame done");
        // }
    }

    void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
        if (moveX == 0 && moveY == 0){
            animator.SetFloat("Speed",0);
        }
        else{
            animator.SetFloat("Speed",moveSpeed);
            
        }
    }

    void Move()
    {
        if (!isEating && !isFighting && !inTrash && !inBoundary){
            rb.velocity = moveDirection * moveSpeed;
        }else {
            rb.velocity = Vector2.zero;
        }
        
    }

    void FlipPlayer()
    {
        if ((moveDirection.x > 0 && !GetComponent<SpriteRenderer>().flipX) ||
            (moveDirection.x < 0 && GetComponent<SpriteRenderer>().flipX))
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }

    // Collecting food
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food")){

            // destroy the food when it's collected
            audioPlayer.Play();
            Destroy(collision.gameObject);

            // updated hunger requirement bar
            foodCollected++;
            foodText.text = "Food: " + foodCollected + "/" + foodRequired;
            if (!isEating)
            {
                StartCoroutine(EatAnimation());
            }


        }
        
        if (collision.gameObject.CompareTag("bush")){
            hiding = true;
            // Debug.Log("bush");
            ResetEnemyDetectionFlags();
            
        } 

        if (collision.gameObject.CompareTag("bin")){
            if (foodCollected == foodRequired){
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                // EnterTrashAnimation();
                // animator.SetTrigger("EnterBin");
                StartCoroutine(EnterTrashAnimation());
                // collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            
            } else {
                binWarningText.gameObject.SetActive(true); 
            }
            
        } 

        if (collision.gameObject.CompareTag("Enemy")){
            isFighting = true;
            StartCoroutine(Caught());
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            
        }

        if (collision.gameObject.CompareTag("House")){
            // saveManager.SwitchToMiniGameScene();
            
            SceneManager.LoadScene ("MiniGame", LoadSceneMode.Additive);
            // Debug.Log("Saving in player");
            // saveManager.SaveGame();
            // SceneManager.LoadScene("MiniGame");
            // hiding = true;
            // Debug.Log("bush");
            // ResetEnemyDetectionFlags();
            
        } 

        if (collision.gameObject.CompareTag("left_bound"))
        {
            Vector2 directionAwayFromBoundary = Vector2.right; 
            float pushDistance = 1f; 
            transform.position += (Vector3)directionAwayFromBoundary * pushDistance;
        }
        else if (collision.gameObject.CompareTag("right_bound"))
        {
            Vector2 directionAwayFromBoundary = Vector2.left; 
            float pushDistance = 1f; 
            transform.position += (Vector3)directionAwayFromBoundary * pushDistance;
        }
        else if (collision.gameObject.CompareTag("top_bound"))
        {
            Vector2 directionAwayFromBoundary = Vector2.down;
            float pushDistance = 1f;
            transform.position += (Vector3)directionAwayFromBoundary * pushDistance;
        }
        else if (collision.gameObject.CompareTag("bottom_bound"))
        {
            Vector2 directionAwayFromBoundary = Vector2.up; 
            float pushDistance = 1f; 
            transform.position += (Vector3)directionAwayFromBoundary * pushDistance;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("bush"))
        {
            hiding = false;
        } else if (collision.gameObject.CompareTag("bin")){
            if(binWarningText != null){
                binWarningText.gameObject.SetActive(false);
            }
            
        }

        if (collision.gameObject.CompareTag("Boundary"))
        {
            inBoundary = false;
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     Debug.Log("entered");
    //     if (collision.gameObject.CompareTag("Boundary"))
    //     {
    //         // rb.velocity = Vector2.zero;
    //         rb.velocity = moveDirection * 0;
    //     }
    // }

    private IEnumerator EatAnimation()
    {
        // Debug.Log("banana");
        isEating = true;
        rb.velocity = moveDirection * 0;
        animator.SetBool("Eat", true);
        hiding = true;

        // Wait for 2 seconds
        yield return new WaitForSeconds(2.4f);

        
        isEating = false; 
        hiding = false;
        animator.SetBool("Eat", false);
    }

    private IEnumerator EnterTrashAnimation()
    {
        inTrash = true;
        animator.SetTrigger("EnterBin");
        // animator.SetBool("EnterBin 0", true);
        rb.velocity = moveDirection * 0;
        hiding = true;

        yield return new WaitForSeconds(2.5f);

        // animator.ResetTrigger("EnterBin");
        // animator.SetBool("EnterBin 0", false);
        // animator.SetBool("Eat", false);
        rb.velocity = moveDirection * 0;
        // inTrash = false; 
        hiding = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator Caught()
    {
        animator.SetBool("Caught", true);
        // hiding = true;

        yield return new WaitForSeconds(2f);

        isFighting = true;
        animator.SetBool("Caught", false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        
        // isEating = false; 
        // hiding = false;
    }


    void ResetEnemyDetectionFlags()
    {
        // Find all objects with the Enemy script
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        
        // Loop through each enemy and reset detection flags
        foreach (Enemy enemy in enemies)
        {
            enemy.ResetDetectionFlags();
            // Debug.Log("name: " + enemy.name);
        }
    }
}
