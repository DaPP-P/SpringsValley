using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    private const float MOVE_SPEED = 7f;

    [SerializeField] private LayerMask dashLayerMask;
    private Rigidbody2D rigidbody2D;
    private Vector3 moveDir;
    private bool isDashButtonDown;
    
    private SwordParent swordParent;
    public GameObject weapon;
    private bool isAttacking;

    public TrailRenderer trailRenderer;

    private void Awake() 
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        
        swordParent = GetComponentInChildren<SwordParent>();
        weapon.SetActive(false);
        isAttacking = false;
        trailRenderer.enabled = false;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        HandleAttack();

        float moveX = 0f;
        float moveY = 0f;

            if(Input.GetKey(KeyCode.W)){
            moveY = +1f;
        }

        if(Input.GetKey(KeyCode.S)){
            moveY = -1f;
        }

        if(Input.GetKey(KeyCode.D)){
            moveX = +1f;
        }

        if(Input.GetKey(KeyCode.A)){
            moveX = -1f;
        }

        moveDir = new Vector3(moveX, moveY).normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDashButtonDown = true;
        }
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = moveDir * MOVE_SPEED;

        if (isDashButtonDown)
        {   
            float dashAmount = 3f;
            Vector3 dashPosition = transform.position + moveDir * dashAmount;
            RaycastHit2D raycastHit2d = Physics2D.Raycast(transform.position,moveDir, dashAmount, dashLayerMask);
            if (raycastHit2d.collider != null) {
                dashPosition = raycastHit2d.point;
            }
            rigidbody2D.MovePosition(dashPosition);
            isDashButtonDown = false;
            StartCoroutine(DashAnimationCoroutine());
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        weapon.SetActive(true);
        swordParent.Attack();

        // Wait for the attack animation duration
        yield return new WaitForSeconds(0.3f);

        weapon.SetActive(false);
        isAttacking = false;
    }

    private IEnumerator DashAnimationCoroutine()
    {
        trailRenderer.enabled = true;

        yield return new WaitForSeconds(0.3f);

        trailRenderer.enabled = false;
    }

}
