using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{

    private SwordParent swordParent;
    private Vector3 lastMoveDir;

    void Awake()
    {
        swordParent = GetComponentInChildren<SwordParent>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleDash();
        if (Input.GetKey(KeyCode.Mouse0)){
            swordParent.Attack();
        }
    }

    private void HandleMovement()
    {
        float speed = 20f;
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
        
        Vector3 targetMovePosition = transform.position + moveDir * speed * Time.deltaTime;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, moveDir, speed * Time.deltaTime);
        if (raycastHit.collider == null || raycastHit.collider.CompareTag("detectArea")) {
            // Can move, nothing in the way.
            lastMoveDir = moveDir;
            transform.position = targetMovePosition;
        } else {
            // Can not move, something in the way.
            Vector3 testMoveDir = new Vector3(moveDir.x, 0f).normalized;
            targetMovePosition = transform.position + testMoveDir * speed * Time.deltaTime;
            raycastHit = Physics2D.Raycast(transform.position, testMoveDir, speed * Time.deltaTime);
            if (raycastHit.collider == null || raycastHit.collider.CompareTag("detectArea")) {
                // Can move horizontally.
                lastMoveDir = moveDir;
                transform.position = targetMovePosition;
            } else {
                // Can not move horizontally.

                // Check if can move vertically.
                testMoveDir = new Vector3(0f,moveDir.y).normalized;
                targetMovePosition = transform.position + testMoveDir * speed * Time.deltaTime;
                raycastHit = Physics2D.Raycast(transform.position, testMoveDir, speed * Time.deltaTime);
                if (raycastHit.collider == null || raycastHit.collider.CompareTag("detectArea")) {
                    // Can move vertically.
                    lastMoveDir = moveDir;
                    transform.position = targetMovePosition;
                } else {
                    // Can not move vertically.
                }
            }
        }
    }

    private bool CanMove(Vector3 dir, float distance) {
        return (Physics2D.Raycast(transform.position, dir, distance).collider == null || Physics2D.Raycast(transform.position, dir, distance).collider.CompareTag("detectArea"));
    }

    private void HandleDash() {
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            float dashDistance = 20f;
            transform.position += lastMoveDir * dashDistance;

        }
    }
}
