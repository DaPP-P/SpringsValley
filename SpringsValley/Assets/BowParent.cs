using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowParent : MonoBehaviour
{
    public SpriteRenderer characterRenderer, weaponRenderer;
    public int offset = 0;

    public bool isAttacking { get; private set; }
    public int leftClickDamageAmount;

    public GameObject arrowPrefab;
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        followMouse();
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }

    /* Follows the mouse */
    public void followMouse()
    {
        if (isAttacking)
            return;

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
        Vector2 scale = transform.localScale;
    }

    public void Attack()
    {
        Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
    }
}
