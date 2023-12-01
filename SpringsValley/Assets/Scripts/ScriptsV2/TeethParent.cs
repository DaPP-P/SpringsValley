using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeethParent : MonoBehaviour
{

    public Transform circleOrigin;
    public float radius;
    private static bool attackBlocked;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectColliders();
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
            HealthSystem2 healthSystem;

            if (attackBlocked == false)
            {
                if (healthSystem = collider.GetComponent<HealthSystem2>())
                {
                healthSystem.GetHit(20, transform.parent.gameObject);
                }
                attackBlocked = true;
                StartCoroutine(DelayAttack());
            }
        }
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(1.0f);
        attackBlocked = false;
    }


}
