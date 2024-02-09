using UnityEngine;
using System.Collections;

public class SkeletonAttackingState : SkeletonBaseState
{

    public EnemySwordParent enemySwordParent;
    private SkeletonStateManager skeleton;
    
    public float moveSpeed = 2.5f;
    float attackRange = 1.0f;
    bool attackDelayed = false;
    float delayStartTime;

    public GameObject player;
    private bool hasLineOfSight;

    public override void EnterState(SkeletonStateManager skeleton)
    {
        Debug.Log("hello from attacking state");
        enemySwordParent = skeleton.GetComponentInChildren<EnemySwordParent>();
        this.skeleton = skeleton;
        player = GameObject.FindGameObjectWithTag("Player");
        hasLineOfSight = true;
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        if (CheckLineOfSight(5))
        {
            strafe();
        }
        else
        {
            skeleton.SwitchState(skeleton.pursuingState);
        }
    }

    private bool CheckLineOfSight(float lineOfSightRange)
    {
        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        int layerMask = ~enemyLayerMask; // Invert the enemy layer mask to exclude it
        RaycastHit2D ray = Physics2D.Raycast(skeleton.transform.position, player.transform.position - skeleton.transform.position, lineOfSightRange, layerMask);
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.CompareTag("Player");
            if(hasLineOfSight)
            {
                Debug.DrawRay(skeleton.transform.position, player.transform.position - skeleton.transform.position, Color.green);
                return true;
            }
            else
            {
                Debug.DrawRay(skeleton.transform.position, player.transform.position - skeleton.transform.position, Color.red);
                return false;
            }
        } else
        {
            return false;
        }
    }

    private void MoveTowardsTarget()
    {
        skeleton.transform.position = Vector3.MoveTowards(skeleton.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }


    private void strafe()
    {
        // Move closer slowly
        if (CheckLineOfSight(attackRange))
        {
            enemySwordParent.Attack();
        }
        else
        {
            MoveTowardsTarget();
        } 
    }

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }
}
