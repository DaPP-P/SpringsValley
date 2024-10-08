using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public float speed = 20f;
    public Rigidbody2D rb;
    private int damageAmount;

    private BowStats bowStats;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
        Invoke("DestroyGameObject", 2f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.name != "Main_Character" && hitInfo.name != "detectOrigin")
        {
            Debug.Log(hitInfo.name);
            damageAmount = 10;

            // Check if the hit object is on the "Solid" layer
            if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Solid"))
            {
                Destroy(gameObject);  // Destroy this game object if hit object is on the "Solid" layer
                return;  // Exit the method early to avoid applying damage or other logic
            }

            // Attempt to get the HealthSystem component from the hit game object
            HealthSystem healthSystem = hitInfo.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                Debug.Log("I hit " + hitInfo.gameObject.name);
                healthSystem.Damage(damageAmount, this.gameObject);
                Destroy(gameObject);
            }
        }
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
