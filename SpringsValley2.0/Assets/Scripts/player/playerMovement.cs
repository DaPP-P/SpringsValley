using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerMovement : MonoBehaviour
{
    private const float MOVE_SPEED = 3f;  // Player movement speed.
    public new Rigidbody2D rigidbody2D;      // Player's Rigidbody2D.
    private Vector3 moveDir;             // Player movement direction.
    public Animator animator;            // Animator for player animations.
    public SpriteRenderer spriteRenderer;
    public bool isAttacking;

    private PlayerHealth playerHealth;

    [SerializeField]
    private float attackDelay = 0.5f; // Delay before attack completes.
    private string currentState;       // Current animation state.

    // Animation States
    const string PLAYER_IDLE = "warrior idle";
    const string PLAYER_RUN = "warrior run";
    const string PLAYER_ATTACK1 = "warrior single swing1";

    // Sound 
    public AudioSource source;
    public AudioClip dashSound;
    public AudioClip deathSound;

    // Dash
    public TrailRenderer trailRenderer; // The players trail.
    private bool isDashButtonDown; // Checks if dash button has been pressed.
    [SerializeField] private LayerMask dashLayerMask;  // For the players dash.
    public float dashAmount = 2f;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Prevent input while attacking.
        if (isAttacking)
        {
            return;
        }

        // Movement input.
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = +1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
            spriteRenderer.flipX = false; // Face right.
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
            spriteRenderer.flipX = true; // Face left.
        }

        // Normalizing move direction.
        moveDir = new Vector3(moveX, moveY).normalized;

        // Animation update based on movement.
        if (moveDir != Vector3.zero)
        {
            ChangeAnimationState(PLAYER_RUN);
        }
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }

        // Dash input.
        if (Input.GetKeyDown(KeyCode.Space))
            isDashButtonDown = true;

        // Attack input.
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        // Update Rigidbody velocity.
        if (!isAttacking) // Prevent movement during attack.
        {
            rigidbody2D.linearVelocity = moveDir * MOVE_SPEED;
        }
        else
        {
            rigidbody2D.linearVelocity = Vector3.zero; // Stop movement.
        }

        
        if (isDashButtonDown)
        {   
            if (playerHealth.CanDecreaseEnergy(10)) 
            {
                Vector3 dashPosition = transform.position + moveDir * dashAmount;
            
                // Uses a ray cast is check if the player will hit a wall before dash. If it will hit 
                // a wall it sets the dash distance to only be up to the wall.
                RaycastHit2D raycastHit2d = Physics2D.Raycast(transform.position, moveDir, dashAmount, dashLayerMask);
                if (raycastHit2d.collider != null) {
                    dashPosition = raycastHit2d.point;
                }

                // Moves the player in the dash direction.
                rigidbody2D.MovePosition(dashPosition);
                isDashButtonDown = false;

                // Starts the dash animation and plays dash sound.
                source.PlayOneShot(dashSound);
                StartCoroutine(DashAnimationCoroutine());
            } else {
                // Reset the dash button down if energy cannot be decreased
                isDashButtonDown = false;
            }
        }
    }


    /* Methods for Attacking */
    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            Vector3 playerPosition = transform.position;
            if (mouseWorldPosition.x < playerPosition.x) {
                spriteRenderer.flipX = true; // Face left.
            } else if (mouseWorldPosition.x > playerPosition.x) {
                spriteRenderer.flipX = false; // Face right.
            }
            ChangeAnimationState(PLAYER_ATTACK1);
            Invoke("AttackComplete", attackDelay); // Reset attack state after delay.
        }
    }

    void AttackComplete()
    {
        isAttacking = false;
    }


    /* Methods for sprite and player state */
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return; // Prevent interrupting the same animation.
        animator.Play(newState);
        currentState = newState;
    }

    public void playerDeath() {
        animator.SetBool("isDead", true);
        source.PlayOneShot(deathSound);
    }

    /* Methods for dashing */
     
    /*
     * Coroutine for enabling the dash animation.
     * Enables it for a certain amount of time then turns it off.
     */
    private IEnumerator DashAnimationCoroutine()
    {
        trailRenderer.enabled = true;

        yield return new WaitForSeconds(0.3f);

        trailRenderer.enabled = false;
    }

}


