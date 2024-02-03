using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordParent : MonoBehaviour
{

    public int leftClickDamageAmount = 10;
    public Vector2 pointerPosition;
    public SpriteRenderer characterRenderer, weaponRenderer;
    public int offset = 0;
    public Animator animator; 
    public float delay = 0.3f;
    private static bool attackBlocked; 

    public Transform circleOrigin;
    public float radius;

    public bool IsAttacking { get; private set; }

    public int attackCount;

    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }

    void Awake()
    {
        attackCount = 0;
    }

    void Start()
    {
        
    }

    void Update()
    {
        // This is messy but i cant be bothered fixing might need to at some point.
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
        Vector2 scale = transform.localScale;

        if (difference.x < 0) {
            characterRenderer.flipX = true;
        }

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
        animator.SetTrigger("Attack");
        IsAttacking = true;
        attackBlocked = true;
        StartCoroutine(DelayAttack());
        attackCount += 1;
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
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
            if (healthSystem = collider.GetComponent<HealthSystem>())
            {
                healthSystem.Damage(leftClickDamageAmount, transform.parent.gameObject);
            }
        }
    }
}
