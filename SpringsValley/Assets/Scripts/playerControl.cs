using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControl : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D body;
    float horizontal;
    float vertical;
    float movementDir;
    public float speed = 10.0f;
    public Vector2 mouseDirection = new Vector2();
    public GameObject weapon;
    public float offset;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        movementDir = Mathf.Sign(horizontal);
        transform.GetComponent<SpriteRenderer>().flipX = movementDir < 0;

        weaponDirection();
        
        if (Input.GetKey(KeyCode.Mouse0))        
        {
            StartCoroutine(hit());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprint();
        }
    }

    private void FixedUpdate() {
        body.velocity = new Vector2(horizontal * speed, vertical * speed);
    }

void weaponDirection()
{
    Vector3 mousePosition = Input.mousePosition;
    mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
    Vector2 direction = new Vector2(
        mousePosition.x - transform.position.x,
        mousePosition.y - transform.position.y
    );
    // Calculate the angle of the direction vector
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    // Snap the angle to the nearest 22.5-degree interval
    float snappedAngle = Mathf.Round(angle / 22.5f) * 22.5f;

    // Convert the snapped angle back to radians
    float snappedAngleRad = snappedAngle * Mathf.Deg2Rad;

    // Update the direction vector based on the snapped angle
    direction = new Vector2(Mathf.Cos(snappedAngleRad), Mathf.Sin(snappedAngleRad));

    direction.Normalize();

    mouseDirection = direction;
    // Check if the angle is greater than 90 degrees or less than -90 degrees
    if (Vector2.Angle(Vector2.right, direction) > 90f)
    {
        // If true, set the player's rotation to the left
        weapon.transform.right = -direction;
        //transform.localScale = new Vector3(-1, 1, 1);
    } else {
        // If false, set the player's rotation to the right
        weapon.transform.right = direction;
        //transform.localScale = new Vector3(1, 1, 1);
    }
}

IEnumerator hit()
{
    print("hit");
    weapon.SetActive(true);

    // Calculate the weapon position based on the player's position and mouse direction
    Vector3 weaponPosition = transform.position + offset * new Vector3(mouseDirection.x, mouseDirection.y, 0f);
    weapon.transform.position = weaponPosition;

    yield return new WaitForSeconds(0.2f);

    weapon.SetActive(false);
}

void sprint(){
    speed = speed * 1.5f;
}

}