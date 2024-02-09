using UnityEngine;

public class SkeletonPursuingState : SkeletonBaseState
{
    public float moveSpeed = 2.5f;
    public Vector3 targetPosition;
    private Vector3 originalPosition;
    private bool playerInRadius;
    private SkeletonStateManager skeleton;
    public GameObject player;
    private bool hasLineOfSight;



    public override void EnterState(SkeletonStateManager skeleton)
    {
        this.skeleton = skeleton;
        player = GameObject.FindGameObjectWithTag("Player");
        hasLineOfSight = true;
        Debug.Log("hello from pursuing state");
        skeleton.exclamationPoint.SetActive(true);
        skeleton.detectionCollider.radius =15f;
        playerInRadius = false;
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        MoveTowardsTarget();

        if (Vector3.Distance(skeleton.transform.position, player.transform.position) <= 1.0f)
        {
            skeleton.SwitchState(skeleton.attackingState);
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

    private void OnTriggerExit2D(Collider2D other)
    {

    }

    
    private bool LocateTarget()
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

    private void MoveTowardsTarget()
    {
        if (LocateTarget())
        {
            // Move towards the target position
            skeleton.transform.position = Vector3.MoveTowards(skeleton.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Handle the case where the player is not in the radius (e.g., stop pursuing, switch to idle state, etc.)
            skeleton.SwitchState(skeleton.idleState);
        }
    }
}
