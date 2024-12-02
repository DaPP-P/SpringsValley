using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private const float MOVE_SPEED = 3f;     // Player movement speed.
    public Rigidbody2D rigidbody2D; // Players rigidbody.
    private Vector3 moveDir; // Player movement direction.
    public Animator animator; // The animator 

    public SpriteRenderer spriteRenderer;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    void Update()
    {

        // Sets movement to zero.
        float moveX = 0f;
        float moveY = 0f;

        // Checks movement direction.
        if(Input.GetKey(KeyCode.W)) {
            moveY = +1f; 
        } 
        if(Input.GetKey(KeyCode.S)) {
            moveY = -1f;
        }
        if(Input.GetKey(KeyCode.D)) {
            moveX = +1f;
            spriteRenderer.flipX = false; // Face right
        }
        if(Input.GetKey(KeyCode.A)) {
            moveX = -1f;
            spriteRenderer.flipX = true; // Face left
        }

        if (moveY != 0 || moveX != 0) {
            animator.SetBool("Running", true);
        } else {
            animator.SetBool("Running", false);
        }

        // normalizes the move direction.
        moveDir = new Vector3(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        rigidbody2D.linearVelocity = moveDir * MOVE_SPEED;

    }
}
