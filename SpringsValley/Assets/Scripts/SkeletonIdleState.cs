using UnityEngine;


//TODO: USE PATH FINDING BACK TO ITS LOCATION.
//TODO: WHEN RETREATING MOVE QUICKER AND CANT TURN FOR FIRST PORTION.

public class SkeletonIdleState : SkeletonBaseState
{
    private bool hasLineOfSight; // Bool to check if line of sight with player.
    public float moveRadius = 2f; // radius that the skeleton can move in.
    public float moveSpeed = 1f; // Skeleton move speed.
    private Vector3 targetPosition; // the idle target position.
    private SkeletonStateManager skeleton; // The skeleton state manager.
    public GameObject player; // The player game object.

    /*
     * Setup needed when SkeletonIdleState is loaded.
     */
    public override void EnterState(SkeletonStateManager skeleton)
    {
        // Initial admin setup.
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("hello from idle state");
        hasLineOfSight = false;
        this.skeleton = skeleton;
        skeleton.exclamationPoint.SetActive(false);

        // TODO: THINK I CAN DELETE!
        skeleton.detectionCollider.radius = 10f;

        // Sets the original position on the first load.
        if (SkeletonStateManager.originalPosition == Vector3.zero) {
            SkeletonStateManager.originalPosition = skeleton.transform.position;
            skeleton.retreating = false;
        }
        
        // Start doing things.
        SetRandomTargetPosition();
        MoveTowardsTarget();
    }

    /*
     * Update Method.
     */
    public override void UpdateState(SkeletonStateManager skeleton)
    {             
        // Check for detection of the player in the idle state
        if (skeleton.CheckLineOfSight(player) && (!skeleton.retreating))
        {
            // Switch to pursuing state if detected
            skeleton.SwitchState(skeleton.pursuingState);
            return;
        }

        // Checks if the skeleton is in retreating state or not and changes
        // its spend depending on if its retreating or its distance from its spawn.
        if (!skeleton.CheckDistanceFromSpawn(3f)) {
            moveSpeed = 1f;
            skeleton.retreating = false;
        } else if ((skeleton.retreating) && (!skeleton.CheckDistanceFromSpawn(10f))) {
            moveSpeed = 2f;
            skeleton.retreating = false;
        } else if (skeleton.retreating)
        {
            moveSpeed = 3.5f;
        }

        /* 
         * Moves the skeleton to a random position in the idle area.
         * If skeleton is at the random position choose a new one otherwise move towards the random position.
         */
        if (Vector3.Distance(skeleton.transform.position, targetPosition) < 0.1f)
            SetRandomTargetPosition();
        else
            MoveTowardsTarget();

        /*
         * Flipping the skeletons sprite renderer.
         */
        Vector3 direction = targetPosition - skeleton.transform.position;
        if (Vector3.Dot(direction, skeleton.transform.right) < 0)
            skeleton.characterRenderer.flipX = true;
        else
            skeleton.characterRenderer.flipX = false;
    }

    // TODO: DON'T USE NEED TO DELETE 
    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
       
    }
   
    /**
     ** Method for setting random position.  
     **/
    void SetRandomTargetPosition()
    {
        // Generate a random point within the moveRadius
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        float randomRadius = Random.Range(0f, moveRadius);
        float newX = SkeletonStateManager.originalPosition.x + Mathf.Cos(randomAngle) * randomRadius;
        float newY = SkeletonStateManager.originalPosition.y + Mathf.Sin(randomAngle) * randomRadius;
        targetPosition = new Vector3(newX, newY, skeleton.transform.position.z);
    }

    /**
     ** Method for moving skeleton to a random position.
     **/
    void MoveTowardsTarget()
    {
        skeleton.transform.position = Vector3.MoveTowards(skeleton.transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
