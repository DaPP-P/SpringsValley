using UnityEngine;

public class SkeletonAttackingState : SkeletonBaseState
{

    public EnemySwordParent enemySwordParent;
    private SkeletonStateManager skeleton;

    public override void EnterState(SkeletonStateManager skeleton)
    {
        Debug.Log("hello from attacking state");
        enemySwordParent = skeleton.GetComponentInChildren<EnemySwordParent>();
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        enemySwordParent.Attack();
        GameObject player = GameObject.FindGameObjectWithTag("Player");        
        if (Vector3.Distance(skeleton.transform.position, player.transform.position) > 6.0f)
        {
            skeleton.SwitchState(skeleton.pursuingState);
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
