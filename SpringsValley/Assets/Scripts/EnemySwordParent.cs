using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordParent : MonoBehaviour
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

    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Attack() 
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Attack");
        IsAttacking = true;
        attackBlocked = true;
        StartCoroutine(DelayAttack());
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
