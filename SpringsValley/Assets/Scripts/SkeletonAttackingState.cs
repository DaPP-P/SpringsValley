using UnityEngine;
using System.Collections;

public class SkeletonAttackingState : SkeletonBaseState
{

    public EnemySwordParent enemySwordParent;
    private SkeletonStateManager skeleton;
    
    bool attackDelayed = false;
    float delayStartTime;

    public override void EnterState(SkeletonStateManager skeleton)
    {
        Debug.Log("hello from attacking state");
        enemySwordParent = skeleton.GetComponentInChildren<EnemySwordParent>();
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        //enemySwordParent.Attack();
        GameObject player = GameObject.FindGameObjectWithTag("Player");        
        if (Vector3.Distance(skeleton.transform.position, player.transform.position) > 2.0f)
        {
            skeleton.SwitchState(skeleton.pursuingState);
            attackDelayed = false;
        } else {
            // If the delay flag is not set, set it and record the start time
            if (!attackDelayed)
            {
                attackDelayed = true;
                delayStartTime = Time.time;
            }

            // Check if 0.3 seconds have passed since the delay started
            if (Time.time - delayStartTime >= 0.3f)
            {
                // Execute the attack after the delay
                enemySwordParent.Attack();
                // Reset the delay flag
                attackDelayed = false;
            }

        }
    }


    private void strafe()
    {
        // Move closer slowly
        // Attack
        // Mive away faster.

        //if (Vector3.Distance(skeleton.transform.position, player.transform.position) > 3.0f)
        //{
        //
        //}
    }

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }
}
