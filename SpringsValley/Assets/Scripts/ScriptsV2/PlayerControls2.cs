using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls2 : MonoBehaviour
{

    Rigidbody2D body;
    float horizontal;
    float vertical;
    float movementDir;
    public float speed = 10.0f;
    public static Vector2 mouseDirection = new Vector2();
    public SpriteRenderer playerSprite, weaponSprite;
    public static Vector2 weaponDirectionCoords = new Vector2();

    private SwordParent2 swordParent2;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        swordParent2 = GetComponentInChildren<SwordParent2>(); // Assuming SwordParent2 is a child of PlayerControls2
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the direction of the player and direction facing
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        movementDir = Mathf.Sign(horizontal);
        playerSprite.flipX = movementDir < 0;

        if (Input.GetKey(KeyCode.Mouse0)) 
        {
            leftHit();
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
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        weaponDirectionCoords = new Vector2(
        mousePosition.x - transform.position.x,
        mousePosition.y - transform.position.y );  

        weaponDirectionCoords.Normalize();
    }

    public void leftHit()
    {
        print("hit");
        swordParent2.Attack();
    }

}
