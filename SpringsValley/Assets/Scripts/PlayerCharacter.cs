using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{

    private SwordParent swordParent;
    private Vector3 lastMoveDir;
    private Vector3 slideDir;
    private float slideSpeed;
    public float originalSpeed;
    private float currentSpeed;

    public GameObject weapon;

    private State state;
    private enum State {
        Normal,
        Combat,
        DodgeRollSliding,
    }

    void Awake()
    {   
        currentSpeed = originalSpeed;
        swordParent = GetComponentInChildren<SwordParent>();
        state = State.Normal;
        weapon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
        case State.Normal:
            HandleMovement();
            HandleSprint();
            HandleState();
            break;
        case State.Combat:
            HandleMovement();
            HandleDodgeRoll();
            HandleState();
            HandleAttack();
            break;
        case State.DodgeRollSliding:
            HandleDodgeRollSliding();
            break;
        }
    }

    private void HandleState() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (state == State.Normal) {
                state = State.Combat;
                weapon.SetActive(true);
            } else if (state == State.Combat) {
                state = State.Normal;
                weapon.SetActive(false);
            }
        }
    }

    private void HandleAttack() {
        if (Input.GetKey(KeyCode.Mouse0)){
            swordParent.Attack();
        }
    }

    private void HandleMovement() {
        float speed = currentSpeed;
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

        Vector3 moveDir = new Vector3(moveX, moveY).normalized;

        if (TryMove(moveDir, speed * Time.deltaTime)) {
            // player is moving.
        } else {
            // player is not moving.
        }
  
    }

    private bool TryMove(Vector3 baesMoveDir, float distance) {
        Vector3 moveDir = baesMoveDir;

        bool canMove = CanMove(moveDir, distance);
        if (!canMove) {
            // Cannot move diagonally.
            moveDir = new Vector3(baesMoveDir.x, 0f).normalized;
            canMove = moveDir.x != 0f && CanMove(moveDir, distance);
            if (!canMove) {
                // Cannot move vertically.
                moveDir = new Vector3(0f, baesMoveDir.y).normalized;
                canMove = moveDir.y != 0f && CanMove(moveDir, distance);
            }
        }

        if (canMove) {
            lastMoveDir = moveDir;
            transform.position += moveDir * distance;
            return true;
        } else {
            return false;
        }
    }

    private bool CanMove(Vector3 dir, float distance) {
        return (Physics2D.Raycast(transform.position, dir, distance).collider == null || Physics2D.Raycast(transform.position, dir, distance).collider.CompareTag("detectArea") || Physics2D.Raycast(transform.position, dir, distance).collider.CompareTag("Player"));
    }

    private void HandleDash() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            
            // Need to do, make it so it does a ray cast so it can dash up to the wall if in the dash distance.

            float dashDistance = 40f;
            TryMove(lastMoveDir, dashDistance);
        }
    }

    private void HandleSprint() {
        if (Input.GetKey(KeyCode.Space)) {
            currentSpeed = originalSpeed * 1.5f;
        } else {
            currentSpeed = originalSpeed;
        }
    }

    private void HandleDodgeRoll() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            state = State.DodgeRollSliding;
            slideDir = lastMoveDir;
            slideSpeed = 40f;
        }
    }

    private void HandleDodgeRollSliding() {
        TryMove(slideDir, slideSpeed * Time.deltaTime);
        transform.position += slideDir * slideSpeed * Time.deltaTime;

        slideSpeed -= slideSpeed * 5f * Time.deltaTime;
        if (slideSpeed < 5f) {
            state = State.Combat;
        }
    }
}
