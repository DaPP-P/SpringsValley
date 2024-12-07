using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float MOVE_SPEED = 3f;  // Player movement speed.
    public Rigidbody2D rigidbody2D;      // Player's Rigidbody2D.
    private Vector3 moveDir;             // Player movement direction.
    public Animator animator;            // Animator for player animations.
    public SpriteRenderer spriteRenderer;
    public bool isAttacking;

    [SerializeField]
    private float attackDelay = 0.5f; // Delay before attack completes.
    private string currentState;       // Current animation state.

    // Animation States
    const string PLAYER_IDLE = "warrior idle";
    const string PLAYER_RUN = "warrior run";
    const string PLAYER_ATTACK1 = "warrior single swing1";

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            ChangeAnimationState(PLAYER_ATTACK1);
            Invoke("AttackComplete", attackDelay); // Reset attack state after delay.
        }
    }

    void AttackComplete()
    {
        isAttacking = false;
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return; // Prevent interrupting the same animation.
        animator.Play(newState);
        currentState = newState;
    }
}
