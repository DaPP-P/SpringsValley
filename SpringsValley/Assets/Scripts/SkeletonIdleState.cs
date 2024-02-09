using UnityEngine;


public class SkeletonIdleState : SkeletonBaseState
{


    private bool hasLineOfSight;


    public float moveRadius = 2f;
    public float moveSpeed = 1f;
    private Vector3 targetPosition;
    private Vector3 originalPosition = new Vector3(-4, -4, 0);
    private SkeletonStateManager skeleton;
    public GameObject player;

    public override void EnterState(SkeletonStateManager skeleton)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("hello from idle state");
        hasLineOfSight = false;
        this.skeleton = skeleton;
        skeleton.exclamationPoint.SetActive(false);
        skeleton.detectionCollider.radius = 10f;
        SetRandomTargetPosition();
        MoveTowardsTarget();
    }

    private bool CheckLineOfSight()
    {
        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        int layerMask = ~enemyLayerMask; // Invert the enemy layer mask to exclude it
        RaycastHit2D ray = Physics2D.Raycast(skeleton.transform.position, player.transform.position - skeleton.transform.position, 12, layerMask);
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

    public override void UpdateState(SkeletonStateManager skeleton)
    {       
        // Check for detection in the idle state
        if (CheckLineOfSight())
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
                    return true;
                }
            }
        }
        return false;
    }
}
