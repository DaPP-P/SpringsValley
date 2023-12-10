using UnityEngine;

public class SkeletonIdleState : SkeletonBaseState
{

    public float moveRadius = 5f;
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private Vector3 originalPosition = new Vector3(-20, -5, 0);
    private SkeletonStateManager skeleton;

    public override void EnterState(SkeletonStateManager skeleton)
    {
        Debug.Log("hello from idle state");
        this.skeleton = skeleton;
        skeleton.exclamationPoint.SetActive(false);
        skeleton.detectionCollider.radius = 20f;
        SetRandomTargetPosition();
        MoveTowardsTarget();

    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        // Check for detection in the idle state
        if (CheckDetection())
        {
            // Switch to pursuing state if detected
            skeleton.SwitchState(skeleton.pursuingState);
            return;
        }

        if (Vector3.Distance(skeleton.transform.position, targetPosition) < 0.1f)
        {
            SetRandomTargetPosition();
        } else {
            MoveTowardsTarget();
        }

        Vector3 direction = targetPosition - skeleton.transform.position;

        if (Vector3.Dot(direction, skeleton.transform.right) < 0)
        {
            skeleton.characterRenderer.flipX = true;
        }
        else
        {
            skeleton.characterRenderer.flipX = false;
        }
        
    }

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }
    
    void SetRandomTargetPosition()
    {
        // Generate a random point within the moveRadius
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        float randomRadius = Random.Range(0f, moveRadius);
        float newX = originalPosition.x + Mathf.Cos(randomAngle) * randomRadius;
        float newY = originalPosition.y + Mathf.Sin(randomAngle) * randomRadius;

        targetPosition = new Vector3(newX, newY, skeleton.transform.position.z);
    }

    void MoveTowardsTarget()
    {
        // Move towards the target position
        skeleton.transform.position = Vector3.MoveTowards(skeleton.transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
    
    private bool CheckDetection()
    {
        // Ensure the detectionGameObject and detectionCollider are not null
        if (skeleton.detectionGameObject != null && skeleton.detectionCollider != null)
        {
            // Check if the detection collider overlaps with the skeleton's position
            Collider2D[] colliders = Physics2D.OverlapCircleAll(skeleton.transform.position, skeleton.detectionCollider.radius);

            foreach (var collider in colliders)
            {
                // Check if the detected object has the "Player" tag
                if (collider.CompareTag("Player"))
                {
                    // You can add more specific conditions here if needed
                    return true;
                }
            }
        }
        return false;
    }
}

