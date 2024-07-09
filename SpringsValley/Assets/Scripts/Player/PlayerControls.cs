using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private WeaponStateManager weaponStateManager; // Reference to WeaponStateManager needed for swapping weapons.
    
    private PlayerHealth playerHealth;
    private const float MOVE_SPEED = 7f;     // Player movement speed.
    [SerializeField] private LayerMask dashLayerMask;  // For the players dash.
    public Rigidbody2D rigidbody2D; // Players rigidbody.
    private Vector3 moveDir; // Player movement direction.
    private bool isDashButtonDown; // Checks if dash button has been pressed.
    private SwordParent swordParent; // Reference to the sword weapon.
    private BowParent bowParent; // Reference to the bow weapon.
    private bool isAttacking; // Bool for it the player is attacking.

    public TrailRenderer trailRenderer; // The players trail.
    public Animator animator; // The animator to play the players trail.

    private float dashAttackLength = .5f;
    public float dashAttackSpeed;

    private bool movementFreeze = false;

    private float dashAttackCounter, dashAttackCoolCounter;

    public AudioSource source;
    public AudioClip dashSound;
    public AudioClip deathSound;

    /*
     * Setup needed when player is Awaken.
     */
    private void Awake() 
    {
        // Iniial admin setup.
        rigidbody2D = GetComponent<Rigidbody2D>();
        isAttacking = false;
        trailRenderer.enabled = false;
        
        // Getting components
        swordParent = GetComponentInChildren<SwordParent>();
        bowParent = GetComponentInChildren<BowParent>();
        weaponStateManager = GetComponentInChildren<WeaponStateManager>();
        playerHealth = GetComponent<PlayerHealth>();        
    }

    /*
     * The update method.
     */ 
    void Update()
    {
        // Checks if Mouse0 or Mouse1 are clicked.
        HandleAttack();

        // Sets movement to zero.
        float moveX = 0f;
        float moveY = 0f;

        // Checks movement direction.
        if(Input.GetKey(KeyCode.W))
            moveY = +1f;
        if(Input.GetKey(KeyCode.S))
            moveY = -1f;
        if(Input.GetKey(KeyCode.D))
            moveX = +1f;
        if(Input.GetKey(KeyCode.A))
            moveX = -1f;

        // normalizes the move direction.
        moveDir = new Vector3(moveX, moveY).normalized;

        // Checks if space has been pressed.
        if (Input.GetKeyDown(KeyCode.Space))
            isDashButtonDown = true;

        // Checks the players direction for the animation.
        CheckPlayerDirection();
    }

    /*
     * Fixed update method, is called a fixed amount of time.
     */
    private void FixedUpdate()
    {
        // Sets the rigidbody velocity in the direction and speed wanted.
        if (!movementFreeze) {
            rigidbody2D.velocity = moveDir * MOVE_SPEED;
        } else {
            rigidbody2D.velocity = gameHelperScript.returnMouseDirection(gameObject) * dashAttackSpeed;

        }
        // If Space pressed make the player dash.
        if (isDashButtonDown && playerHealth.currentEnergy >= 10)
        {   
            playerHealth.currentEnergy -= 10;
            float dashAmount = 3f;
            Vector3 dashPosition = transform.position + moveDir * dashAmount;
            
            // Uses a ray cast is check if the player will hit a wall before dash. If it will hit 
            // a wall it sets the dash distance to only be up to the wall.
            RaycastHit2D raycastHit2d = Physics2D.Raycast(transform.position,moveDir, dashAmount, dashLayerMask);
            if (raycastHit2d.collider != null) {
                dashPosition = raycastHit2d.point;
            }

            // Moves the player in the dash direction.
            rigidbody2D.MovePosition(dashPosition);
            isDashButtonDown = false;

            // Starts the dash animation and play dash sound.
            source.PlayOneShot(dashSound);

            StartCoroutine(DashAnimationCoroutine());
        }
    }

    /*
     * Method for checking the direction of the player.
     * Used to change the animation trigger for the player.
     */
    private void CheckPlayerDirection()
    {
        Vector3 vel = transform.rotation * rigidbody2D.velocity;
        if (vel.y > 0 && vel.x == 0) {
            animator.SetTrigger("GoUp");
        } else if (vel.y < 0 && vel.x == 0) {
            animator.SetTrigger("GoDown");
        }
        if (vel.x > 0) {
            animator.SetTrigger("GoRight");
        } else if (vel.x < 0) {
            animator.SetTrigger("GoLeft");
        }
    }

    public void callDashAndAttack(){
        StartCoroutine(dashingWhileAttacking());
    }

    /* 
     * Handle the player moving forward while attacking.
     */
    public IEnumerator dashingWhileAttacking(){
        Debug.Log(gameHelperScript.returnMouseDirection(gameObject));
        movementFreeze = true;
        // Wait for 0.1 seconds
        yield return new WaitForSeconds(.1f);

        // Stop the dash
        rigidbody2D.velocity = Vector2.zero;
        movementFreeze = false;
    }

    /* 
     * Method for checking if the player is attacking.
     */
    private void HandleAttack()
    {
         // If left click, call the AttackCoroutine.
        if (Input.GetKey(KeyCode.Mouse0) && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }

         // If right click, call the SpecialAttackCoroutine.
        if (Input.GetKey(KeyCode.Mouse1) && !isAttacking)
        {
            StartCoroutine(SpecialAttackCoroutine());

        }
    }

    /*
     * Coroutine for a normal attack. 
     * Sets the isAttacking bool to true, calls attack, waits a time, sets the isAttacking bool to false.
     */
    private IEnumerator AttackCoroutine()
    {   
        isAttacking = true;
        weaponStateManager.Attack();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    /*
     * Coroutine for a special attack. 
     * Sets the isAttacking bool to true, calls special attack, waits a time, sets the isAttacking bool to false.
     */
    private IEnumerator SpecialAttackCoroutine()
    {
        isAttacking = true;
        weaponStateManager.SpecialAttack();
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

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

    public void playerDeath() {
        animator.SetBool("isDead", true);
        source.PlayOneShot(deathSound);
    }

}
