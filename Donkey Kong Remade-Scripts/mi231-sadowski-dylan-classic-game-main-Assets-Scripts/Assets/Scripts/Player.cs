using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private Collider2D[] results;
    private Vector2 direction;
    private Vector2 jumpVelocity;
    private bool grounded;
    private bool climbing;
    private bool canUseHammer = false;
    private bool hasJumped = false;
    private bool jumpedThroughLadder = false;
    private bool canMove = true;
    private float horizontalInput;
    private float initialHorizontalDirection; 

    private Hammer attachedHammer;
    private bool isHoldingHammer = false;
    private Vector2 savedVelocity;
    private int barrelCollisions = 0;
    private LivesRemaining livesRemaining;

    public float moveSpeed = 1f;
    public float climbSpeed = 1f;
    public float jumpStrength = 1f;
    public GameObject PlayerSpawner;
    public GameObject barrelPrefab;
    public GameObject Hammer;

    public float hammerDuration = 10f;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        results = new Collider2D[4];
    }

    private void Start()
    {
        if (PlayerSpawner == null)
        {
            Debug.LogError("PlayerSpawner is not assigned in the Player script!");
        }

        livesRemaining = FindObjectOfType<LivesRemaining>();

        StartCoroutine(EnablePlayerMovement(2f));
        StartCoroutine(StartBarrelSpawning(3f));
        StartCoroutine(EnableHammer(hammerDuration)); 
    }

    private void Update()
    {
        if (!canUseHammer)
            return;

        CheckCollision();

        horizontalInput = Input.GetAxis("Horizontal");

        float verticalInput = isHoldingHammer ? 0f : Input.GetAxis("Vertical");

        if (!isHoldingHammer)
        {
            if (!climbing)
            {
                direction.x = horizontalInput * moveSpeed;
            }
            else
            {
                direction.x = horizontalInput * moveSpeed;
            }

            if (climbing && !jumpedThroughLadder)
            {
                direction.y = verticalInput * climbSpeed;
            }
            else if (!climbing)
            {
                if (grounded && Input.GetButtonDown("Jump") && !Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ladder")))
                {
                    if (!hasJumped) 
                    {
                        direction = Vector2.up * jumpStrength;
                        hasJumped = true;
                        jumpedThroughLadder = false;
                        initialHorizontalDirection = horizontalInput; 
                        canMove = false; 
                    }
                }
                else if (!grounded)
                {
                    if (hasJumped)
                    {
                        direction.x = Mathf.Clamp(horizontalInput * moveSpeed, initialHorizontalDirection * moveSpeed, initialHorizontalDirection * moveSpeed);
                    }

                    direction += Physics2D.gravity * Time.deltaTime;
                }
            }
        }
        else 
        {
            direction = Vector2.zero;
        }

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
            hasJumped = false;
            canMove = true; 
        }

        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        if (canUseHammer && Input.GetKeyDown(KeyCode.F))
        {
            if (!isHoldingHammer)
            {
                UseHammer(transform);
            }
            else
            {
                DropHammer();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!canUseHammer)
            return;

        if (canMove)
        {
            if (hasJumped)
            {
                playerRigidbody.velocity = jumpVelocity;
            }
            else
            {
                playerRigidbody.velocity = direction;
            }
        }
        else
        {
            playerRigidbody.velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            enabled = false;
            GameManager.instance.LevelComplete();
        }
        else if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Barrier"))
        {
            if (isHoldingHammer)
            {
                DropHammer();
            }

            RespawnPlayer();
            livesRemaining.UpdateCountdown();

            DestroyAllObstacles();
        }
        else if (collision.gameObject.CompareTag("Barrel"))
        {
            barrelCollisions++;

            if (barrelCollisions >= 3)
            {
                SceneManager.LoadScene("GameOver");
            }

            if (isHoldingHammer)
            {
                DropHammer();
            }

            GameObject[] hammers = GameObject.FindGameObjectsWithTag("Hammer");
            foreach (GameObject hammer in hammers)
            {
                if (hammer.GetComponent<Hammer>() == attachedHammer)
                {
                    DropHammer(); 
                }
                else
                {
                    Destroy(hammer);
                }
            }
        }
        else if (collision.gameObject.CompareTag("Hammer"))
        {
            UseHammer(transform); 
            Destroy(collision.gameObject); 
        }
    }


    private void CheckCollision()
    {
        grounded = false;
        climbing = false;

        Vector2 size = playerCollider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, results);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = results[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < transform.position.y;
                Physics2D.IgnoreCollision(playerCollider, results[i], !grounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;
            }
        }

        if (!grounded && jumpedThroughLadder)
        {
            climbing = false;
        }

        if (climbing && !grounded && Input.GetButtonDown("Jump") && !isHoldingHammer)
        {
            direction = Vector2.up * jumpStrength;
            hasJumped = true; 
            jumpedThroughLadder = true; 
        }
    }

    private void UseHammer(Transform playerTransform)
    {
        if (Hammer == null)
        {
            Debug.LogWarning("Hammer prefab not set!");
            return;
        }

        GameObject hammerObject = Instantiate(Hammer, playerTransform.position, Quaternion.identity);
        attachedHammer = hammerObject.GetComponent<Hammer>(); 
        if (attachedHammer != null)
        {
            attachedHammer.PickUp(playerTransform); 
            isHoldingHammer = true;

            hasJumped = false;
            jumpedThroughLadder = false;
            climbing = false;
        }
        else
        {
            Debug.LogWarning("No Hammer component found on the instantiated hammer object.");
        }
    }

    private void DropHammer()
    {
        if (attachedHammer != null)
        {
            Destroy(attachedHammer.gameObject); 
            attachedHammer = null; 
            isHoldingHammer = false;

            climbing = true; 
        }
    }

    private IEnumerator EnableHammer(float duration)
    {
        yield return new WaitForSeconds(duration);
        canUseHammer = true;
    }

    private IEnumerator EnablePlayerMovement(float delay)
    {
        yield return new WaitForSeconds(delay);
        canUseHammer = true;
    }

    private IEnumerator StartBarrelSpawning(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<Spawner>().enabled = true;
    }

    private void RespawnPlayer()
    {
        transform.position = PlayerSpawner.transform.position;
    }

    private void DestroyAllObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
    }
}