using UnityEngine;
using System.Collections;
using Pathfinding;

public class SkeletonAttackingState : SkeletonBaseState
{

    public EnemySwordParent enemySwordParent;
    private SkeletonStateManager skeleton;
    
    public float moveSpeed = 2.5f;
    float attackRange = 1.0f;
    bool attacking = false;
    float attackDelay;
    float delayStartTime;

    public GameObject player;
    public GameObject followObject;

    private bool hasLineOfSight;

    private float lineOfSightRange = 5f;

    // Stuff needed for circling the player
    public float circleRadius = 5f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private float angle;

    public float lineOfSightTimer;

    Seeker seeker;
    Rigidbody2D rb;

    /*
     * Setup needed when SkeletonAttackingState is loaded.
     */
    public override void EnterState(SkeletonStateManager skeleton)
    {
        // Initial admin setup.
        Debug.Log("hello from attacking state");
        enemySwordParent = skeleton.GetComponentInChildren<EnemySwordParent>();
        this.skeleton = skeleton;
        player = GameObject.FindGameObjectWithTag("Player");
        lineOfSightTimer = Time.time;

        // Gets the needed components.
        seeker = skeleton.GetComponent<Seeker>();
        rb = skeleton.GetComponent<Rigidbody2D>();
    }

    /*
     * Update Method.
     */
    public override void UpdateState(SkeletonStateManager skeleton)
    {

        // If the skeleton can't see the player or the player is to far away go back to the pursuing state.
        // Otherwise attack.
        if (!skeleton.CheckLineOfSight(player) || (Vector3.Distance(skeleton.transform.position, player.transform.position) > 3.5f)) {
            skeleton.SwitchState(skeleton.pursuingState);
        } else {
            attackMethod();
        }
    }

    /*
     * Method for the skeleton to attack the player.
     */
    private void attackMethod()
    {
        // Calculate the direction vector from the skeleton to the player
        Vector3 direction = (player.transform.position - skeleton.transform.position).normalized;

        // Move the skeleton along the direction vector
        skeleton.transform.position += direction * moveSpeed * Time.deltaTime;

        Debug.Log("Skeleton should attack!");
        enemySwordParent.Attack();
        attacking = false;
        attackDelay = 0f;
    }

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }
}
