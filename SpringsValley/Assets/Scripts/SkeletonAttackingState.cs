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

        // TODO: MIGHT NOT NEED.
        hasLineOfSight = true;

        // Gets the needed components.
        seeker = skeleton.GetComponent<Seeker>();
        rb = skeleton.GetComponent<Rigidbody2D>();
    }

    /*
     * Update Method.
     */
    public override void UpdateState(SkeletonStateManager skeleton)
    {

        // If the skeleton can't see the player go back to the pursuing state.
        if (!CheckLineOfSight()) {
            skeleton.SwitchState(skeleton.pursuingState);
        }

        // dash and swing in the players direction
        if (!attacking && (Time.time - attackDelay > 0.5f)) {
            attacking = true;

        }

        //// if ((Vector3.Distance(skeleton.transform.position, player.transform.position) <= 15.0f) && LocateTarget(lineOfSightRange))
        //// {
        ////     circleRadius = Vector3.Distance(skeleton.transform.position, player.transform.position);
        ////     AttackMode();
        //// }
        //// else
        //// {
        ////     skeleton.SwitchState(skeleton.pursuingState);
        //// }
    }

    /*
     * Method for the skeleton to attack the player.
     */
    private void attackMethod()
    {
        Debug.Log("Skeleton should attack!");
        attacking = false;
        attackDelay = 0f;
    }

    /*
     * Method to check if the skeleton has a line of sight with the player
     */
    private bool CheckLineOfSight()
    {
        // Invert the enemy layer mask to exclude it so it doesn't get in the way of the LoS
        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        int layerMask = ~enemyLayerMask;
        
        /*
         * Sets up three ray casts. One aiming at the center of the player,
         * one at the bottom of the player and one at the top of the player.
         */
        RaycastHit2D rayCenter = Physics2D.Raycast(skeleton.transform.position, player.transform.position - skeleton.transform.position, 12, layerMask);
        Vector3 modifiedDirectionTop = (player.transform.position + Vector3.up * 0.5f) - skeleton.transform.position;
        RaycastHit2D rayTop = Physics2D.Raycast(skeleton.transform.position, modifiedDirectionTop, 12, layerMask);
        Vector3 modifiedDirectionBottom = (player.transform.position + Vector3.up * -0.5f) - skeleton.transform.position;
        RaycastHit2D rayBottom = Physics2D.Raycast(skeleton.transform.position, modifiedDirectionBottom, 12, layerMask);

        // Checks center ray cast if hits returns true.
        if ((rayCenter.collider != null) && (rayCenter.collider.CompareTag("Player")))
        {
            Debug.DrawRay(skeleton.transform.position, player.transform.position - skeleton.transform.position, Color.green);
            return true;
        } else {
            Debug.DrawRay(skeleton.transform.position, player.transform.position - skeleton.transform.position, Color.red);
        }

        // Checks top ray cast if hits returns true.
        if ((rayTop.collider != null) && rayTop.collider.CompareTag("Player"))
        {
            Debug.DrawRay(skeleton.transform.position, modifiedDirectionTop, Color.green);
            return true;
            } else {
            Debug.DrawRay(skeleton.transform.position, modifiedDirectionTop, Color.red);
        }

        // Checks bottom ray cast if hits returns true.
        if ((rayBottom.collider != null) && rayBottom.collider.CompareTag("Player"))
        {
            Debug.DrawRay(skeleton.transform.position, modifiedDirectionBottom, Color.green);
            return true;
        } else {
            Debug.DrawRay(skeleton.transform.position, modifiedDirectionBottom, Color.red);
        }

        // returns false if no hits.
        return false;
    }

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }
}
