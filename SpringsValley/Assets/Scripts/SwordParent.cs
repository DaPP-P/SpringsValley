using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordParent : MonoBehaviour
{

    public SpriteRenderer characterRenderer, weaponRenderer;
    public int offset = 0;

    public int attackCount;
    public float delay = 0.4f;
    private bool attackBlocked;
    
    public int leftClickDamageAmount;
    public int rightClickDamageAmount;
    public int damageAmount;

    public Animator animator;
    public bool isAttacking { get; private set; }
    public Transform circleOrigin;
    public float radius;

    private List<GameObject> hitObjects = new List<GameObject>();

    void Awake()
    {
        attackCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        followMouse();
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
        hitObjects.Clear();
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


    public void SuperAttack()
    {
        if (attackBlocked)
            return;

        damageAmount = rightClickDamageAmount;
        animator.SetTrigger("StabAttack");

        attackBlocked = true;
        isAttacking = true;

        StartCoroutine(DelayAttack(delay*1.5f));
    }

    public void Attack()
    {
        if (attackBlocked)
            return;

        damageAmount = leftClickDamageAmount;
        if (attackCount%2 == 0) {
            animator.SetTrigger("AttackDown");
        } 
        else 
        {
            animator.SetTrigger("AttackUp");
        }

        attackBlocked = true;
        isAttacking = true;

        attackCount += 1;

        StartCoroutine(DelayAttack(delay));
        
    }

    private IEnumerator DelayAttack(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        attackBlocked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            HealthSystem healthSystem;
            if ((healthSystem = collider.GetComponent<HealthSystem>()) != null && !hitObjects.Contains(collider.gameObject))
            {
                Debug.Log("I hit " + collider.gameObject.name);
                healthSystem.Damage(damageAmount, transform.parent.gameObject);
                hitObjects.Add(collider.gameObject);
   
            }
        }
    }

}
