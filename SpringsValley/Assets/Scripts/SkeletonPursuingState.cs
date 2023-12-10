using UnityEngine;

public class SkeletonPursuingState : SkeletonBaseState
{
    public float moveSpeed = 5f;
    public Vector3 targetPosition;
    private Vector3 originalPosition;
    private bool playerInRadius;
    private SkeletonStateManager skeleton;


    public override void EnterState(SkeletonStateManager skeleton)
    {
        this.skeleton = skeleton;
        Debug.Log("hello from pursuing state");
        skeleton.exclamationPoint.SetActive(true);
        skeleton.detectionCollider.radius = 30f;
        playerInRadius = false;
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        LocateTarget();
        MoveTowardsTarget();

        if (Vector3.Distance(skeleton.transform.position, targetPosition) <= 6.0f)
        {
            skeleton.SwitchState(skeleton.attackingState);
        }
    }

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {

    }

    
    private void LocateTarget()
    {
        // Ensure the detectionGameObject and detectionCollider are not null
        if (skeleton.detectionGameObject != null && skeleton.detectionCollider != null)
        {
            // Check if the detection collider overlaps with the skeleton's position
            Collider2D[] colliders = Physics2D.OverlapCircleAll(skeleton.transform.position, skeleton.detectionCollider.radius);

            // Assume the player is not in the radius until proven otherwise
            playerInRadius = false;

            foreach (var collider in colliders)
            {
                // Check if the detected object has the "Player" tag
                if (collider.CompareTag("Player"))
                {
                    Vector3 playerPosition = collider.transform.position;
                    Vector3 offset = (skeleton.transform.position - playerPosition).normalized * 4.0f;
                    targetPosition = playerPosition;
                    playerInRadius = true;
                    break; // Exit the loop if a player is found
                }
            }
        }
        else
        {
            playerInRadius = false;
        }
    }

    private void MoveTowardsTarget()
    {
        if (playerInRadius)
        {
            // Move towards the target position
            skeleton.transform.position = Vector3.MoveTowards(skeleton.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Handle the case where the player is not in the radius (e.g., stop pursuing, switch to idle state, etc.)
            skeleton.SwitchState(skeleton.idleState);
        }
    }
}
