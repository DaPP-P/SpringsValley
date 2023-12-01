using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    Rigidbody2D body;
    float horizontal;
    float vertical;
    float movementDir;

    public float defaultSpeed = 10.0f;
    private float speed;

    public static Vector2 mouseDirection = new Vector2();
    public SpriteRenderer playerSprite, weaponSprite;
    public static Vector2 weaponDirectionCoords = new Vector2();

    private SwordParent swordParent;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        swordParent = GetComponentInChildren<SwordParent>();
        speed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the direction of the player and direction facing
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        movementDir = Mathf.Sign(horizontal);
        playerSprite.flipX = weaponDirectionCoords.x < 0;

        if (Input.GetKey(KeyCode.Mouse0)) 
        {
            leftHit();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprint();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ResetSpeed();
        }

    }

    // Used to update the direction of the player
    private void FixedUpdate() 
    {
        body.velocity = new Vector2(horizontal * speed, vertical * speed);
        weaponDirection();
    }

    void weaponDirection()
    {
        if(swordParent.IsAttacking)
            return;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        weaponDirectionCoords = new Vector2(
        mousePosition.x - transform.position.x,
        mousePosition.y - transform.position.y );  

        weaponDirectionCoords.Normalize();
    }

    public void leftHit()
    {
        swordParent.Attack();
    }

    void sprint()
    {
        speed = speed * 1.5f;
    }

    void ResetSpeed()
    {
        speed = defaultSpeed;
    }

}
