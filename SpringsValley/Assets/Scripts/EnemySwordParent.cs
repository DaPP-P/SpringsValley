using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordParent : MonoBehaviour
{

    public SkeletonStateManager SkeletonStateManager;


    public int leftClickDamageAmount = 10;

    public Vector2 pointerPosition;
    public SpriteRenderer characterRenderer, weaponRenderer;
    public int offset = 0;
    public Animator animator; 
    public float delay = 2.0f;
    private static bool attackBlocked; 

    public Transform circleOrigin;
    public float radius;

    public float rotation_z;
    public Vector2 scale;

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
        if (IsAttacking)
            return;

        if (SkeletonStateManager == null)
        {
            // Do nothing.
        }
        else if (SkeletonStateManager.currentState.GetType() == typeof(SkeletonPursuingState) || SkeletonStateManager.currentState.GetType() == typeof(SkeletonAttackingState))
        {
            DisplayWeapon();
        } 
        else  if (SkeletonStateManager.currentState.GetType() == typeof(SkeletonIdleState))
        {
            ResetSwordPosition();
        }
    }


    public void DisplayWeapon()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 difference = player.transform.position - transform.position;
            difference.Normalize();
            rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);

            Vector2 scale = transform.localScale;

            if (Mathf.Abs(rotation_z) > 90)
            {
                scale.y = -1;
            }
            else if (Mathf.Abs(rotation_z) < 90)
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
        else
        {
            Debug.LogWarning("Player not found in the scene!");
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
                print("hit");
            }
        }
    }

    public void ResetSwordPosition() 
    {
        Vector3 defaultPosition = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(defaultPosition);
    }

}
