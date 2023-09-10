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


    }

    private void FixedUpdate() {
        body.velocity = new Vector2(horizontal * speed, vertical * speed);
    }

}
