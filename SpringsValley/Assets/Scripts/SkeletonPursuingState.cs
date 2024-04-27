using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//TODO: MAKE TARGET POSITION UPDATE EVERY X SECS INSTEAD OF JUST WHEN AT END POSITION.

public class SkeletonPursuingState : SkeletonBaseState
{
    public float moveSpeed = 2.5f;
    public Vector3 targetPosition;
    private Vector3 originalPosition;
    private SkeletonStateManager skeleton;
    public GameObject player;
    public GameObject followObject;
    public GameObject distantFollowObject;

    public Transform target;
    public float speed = 400;

    // How close the skeleton needs to be to find a new path.
    public float nextWayPointDistance = 8f;
    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;

    public float lineOfSightTimer;
    Seeker seeker;
    Rigidbody2D rb;


    private float updateTargetTimer = 0f;
    public float updateTargetInterval = 0.2f;

    /*
     * Setup needed when SkeletonPursuingState is loaded.
     */
    public override void EnterState(SkeletonStateManager skeleton)
    {
        // initial admin setup.
        this.skeleton = skeleton;
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        Debug.Log("hello from pursuing state");
        skeleton.exclamationPoint.SetActive(true);
        lineOfSightTimer =  Time.time;
        reachedEndOfPath = false;

        // Gets the needed components.
        seeker = skeleton.GetComponent<Seeker>();
        rb = skeleton.GetComponent<Rigidbody2D>();

        // Starts to figure out where the skeleton needs to go.
        skeleton.StartCoroutine(UpdatePathRepeatedly());   
    }

    /*
     * Coroutine to keep the path updated repeatedly.
     */
    IEnumerator UpdatePathRepeatedly()
    {
        UpdatePath();
        yield return new WaitForSeconds(0.5f); 
    }

    /*
     * Updates the path for pathfinding.
     */
    void UpdatePath()
    {
        if (seeker.IsDone() && !reachedEndOfPath)
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    /*
     * Call back when path finding is complete.
     */
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    /* 
     * Update Method.
     */
    public override void UpdateState(SkeletonStateManager skeleton)
    {

       if (path == null)
        {
            return;
        }

        // Check if the skeleton has gone far from its spawn location.
        if (skeleton.CheckDistanceFromSpawn(20f))
        {
            skeleton.retreating = true;
            skeleton.SwitchState(skeleton.idleState);
        }

        // Checks the line of sight.
        // If line of sight is true resets the line of sight timer.
        if (skeleton.CheckLineOfSight(player))
        {
            lineOfSightTimer = Time.time;
        }
        else if (!skeleton.CheckLineOfSight(player)) {
            // If there is no line of sight for eight seconds switches sates to idle state.
            if (Time.time - lineOfSightTimer > 5f) {
                skeleton.SwitchState(skeleton.idleState);
            }
        }

        // Checks if the skeleton is close to the player.
        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            skeleton.SwitchState(skeleton.attackingState);
            return;
        } else 
        {
            reachedEndOfPath = false;
        }

        // Calculate direction and force towards the current waypoint
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        // Adds force to the player.
        rb.AddForce(force);
        
         // Check distance to current waypoint, and move to the next if reached.
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWayPointDistance)
        {
            currentWayPoint ++;
        } 
    }

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {

    }

}
