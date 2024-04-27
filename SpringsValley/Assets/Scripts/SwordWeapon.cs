using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MonoBehaviour
{

    public SpriteRenderer characterRenderer, weaponRenderer;
    public int offset = 0;

    public int attackCount;
    public bool attackBlocked;

    public Animator animator;


    void Awake()
    {
        attackCount = 0;
        //animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        followMouse();
    }

    /* Follows the mouse */
    private void followMouse()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
        Vector2 scale = transform.localScale;

        if(Mathf.Abs(rotation_z) > 90)
        {
            scale.y = -1;
        }else if(Mathf.Abs(rotation_z) < 90)
        {
            scale.y = 1;
        }

        transform.localScale = scale;
        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
        } 
        else 
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }
    }

    public void Attack()
    {
        if (attackBlocked)
            return;

        if (attackCount%2 == 0) {
        animator.SetTrigger("AttackDown");
        } 
        else 
        {
            animator.SetTrigger("AttackUp");
        }
        
        attackCount += 1;
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(0.3f);
        attackBlocked = false;
    }
}
