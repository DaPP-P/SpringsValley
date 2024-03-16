using UnityEngine;
using System.Collections;

public class SkeletonAttackingState : SkeletonBaseState
{

    public EnemySwordParent enemySwordParent;
    private SkeletonStateManager skeleton;
    
    public float moveSpeed = 2.5f;
    float attackRange = 1.0f;
    bool attackDelayed = false;
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

    public override void EnterState(SkeletonStateManager skeleton)
    {
        Debug.Log("hello from attacking state");
        enemySwordParent = skeleton.GetComponentInChildren<EnemySwordParent>();
        this.skeleton = skeleton;
        player = GameObject.FindGameObjectWithTag("Player");
        followObject = GameObject.FindGameObjectWithTag("FollowObject");
        hasLineOfSight = true;
    }

    public override void UpdateState(SkeletonStateManager skeleton)
    {
        if ((Vector3.Distance(skeleton.transform.position, player.transform.position) <= 15.0f) && LocateTarget(lineOfSightRange))
        {
            circleRadius = Vector3.Distance(skeleton.transform.position, player.transform.position);
            AttackMode();
        }
        else
        {
            skeleton.SwitchState(skeleton.pursuingState);
        }
    }

    private bool LocateTarget(float range)
    {
        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        int layerMask = ~enemyLayerMask; // Invert the enemy layer mask to exclude it
        
        RaycastHit2D rayCenter = Physics2D.Raycast(skeleton.transform.position, player.transform.position - skeleton.transform.position, range, layerMask);

        Vector3 modifiedDirectionTop = (player.transform.position + Vector3.up * 0.5f) - skeleton.transform.position;
        RaycastHit2D rayTop = Physics2D.Raycast(skeleton.transform.position, modifiedDirectionTop, range, layerMask);

        Vector3 modifiedDirectionBottom = (player.transform.position + Vector3.up * -0.5f) - skeleton.transform.position;
        RaycastHit2D rayBottom = Physics2D.Raycast(skeleton.transform.position, modifiedDirectionBottom, range, layerMask);

        if ((rayCenter.collider != null) && (rayCenter.collider.CompareTag("Player")))
        {
            Debug.DrawRay(skeleton.transform.position, player.transform.position - skeleton.transform.position, Color.green);
            return true;
        } else {
            Debug.DrawRay(skeleton.transform.position, player.transform.position - skeleton.transform.position, Color.red);
        }

        if ((rayTop.collider != null) && rayTop.collider.CompareTag("Player"))
        {
            Debug.DrawRay(skeleton.transform.position, modifiedDirectionTop, Color.green);
            return true;
        } else {
            Debug.DrawRay(skeleton.transform.position, modifiedDirectionTop, Color.red);
        }

        if ((rayBottom.collider != null) && rayBottom.collider.CompareTag("Player"))
        {
            Debug.DrawRay(skeleton.transform.position, modifiedDirectionBottom, Color.green);
            return true;
        } else {
            Debug.DrawRay(skeleton.transform.position, modifiedDirectionBottom, Color.red);
        }
        return false;
    }

    private bool LocateFollow(float range)
    {
        Debug.Log("Should be follwing follow object");
        Debug.Log(followObject.name);
        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        int layerMask = ~enemyLayerMask; // Invert the enemy layer mask to exclude it

        RaycastHit2D rayFollow = Physics2D.Raycast(skeleton.transform.position, followObject.transform.position - skeleton.transform.position, range, layerMask);
        
        if ((rayFollow.collider != null) && rayFollow.collider.CompareTag("FollowObject"))
        {
            Debug.DrawRay(skeleton.transform.position, followObject.transform.position - skeleton.transform.position, Color.green);
            return true;
        } else {
            Debug.DrawRay(skeleton.transform.position, followObject.transform.position - skeleton.transform.position, Color.red);
        }
        return false;
    }


    private void MoveTowardsTarget()
    {
        if (LocateTarget(lineOfSightRange))
        {
            // Move towards the target position
            skeleton.transform.position = Vector3.MoveTowards(skeleton.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        } else if (LocateFollow(lineOfSightRange))
        {
            skeleton.transform.position = Vector3.MoveTowards(skeleton.transform.position, followObject.transform.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Handle the case where the player is not in the radius (e.g., stop pursuing, switch to idle state, etc.)
            skeleton.SwitchState(skeleton.idleState);
        }
    }

    private void AttackMode()
{
    Debug.Log("We are in attack mode");
    angle += Time.deltaTime * 100; // update angle
    Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up; // calculate direction from center - rotate the up vector Angle degrees clockwise
    skeleton.transform.position = player.transform.position + direction * circleRadius; // update position based on center, the direction, and the radius (which is a constant)
}

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }
}
