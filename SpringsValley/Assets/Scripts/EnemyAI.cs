using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float moveRadius = 5f;
    public float detectRadius = 10f;
    public float moveSpeed = 2f;
    public bool canMove = true;
    public Vector3 targetPosition;
    public Transform radiusOrigin;
    public Transform detectOrigin;

    private Vector3 originalPosition;

    public GameObject ExclamationPoint;

    public bool attackMode = false;
    public float attackRange = 0.5f;
    public bool canAttack = false;

    public SpriteRenderer characterRenderer;

    private EnemySwordParent enemySwordParent;


    void Start()
    {
        // Initial random target position within the moveRadius
        SetRandomTargetPosition();
        originalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // Add a Rigidbody2D component if it's not already present
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
            rb2d.isKinematic = true;
        }

        CircleCollider2D circleCollider = GetComponentInChildren<CircleCollider2D>();
        circleCollider.radius = detectRadius;

        enemySwordParent = GetComponentInChildren<EnemySwordParent>();
    }

    void Update()
    {
        // Check if the character has reached the target position
        if (attackMode == false) {
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // Set a new random target position within the moveRadius
                SetRandomTargetPosition();
            }
        }

    Vector3 direction = targetPosition - transform.position;

    if (Vector3.Dot(direction, transform.right) < 0)
    {
        characterRenderer.flipX = true;
    }
    else
    {
        characterRenderer.flipX = false;
    }


        if (canAttack){
            enemySwordParent.Attack();
        }

        // Move towards the target position
        if (canMove)
            MoveTowardsTarget();
    }

    void SetRandomTargetPosition()
    {
        // Generate a random point within the moveRadius
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        float randomRadius = Random.Range(0f, moveRadius);
        float newX = originalPosition.x + Mathf.Cos(randomAngle) * randomRadius;
        float newY = originalPosition.y + Mathf.Sin(randomAngle) * randomRadius;

        targetPosition = new Vector3(newX, newY, transform.position.z);
    }

    void MoveTowardsTarget()
    {
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag =="Player")
        {
            attackMode = true;
            //Debug.Log("Player entered detection area." + other.transform.position);
            ExclamationPoint.SetActive(true);
            detectRadius = 20f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag =="Player")
        {
            targetPosition = other.transform.position;
            if (Vector3.Distance(transform.position, targetPosition) <= 3f)
            {
                canAttack = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        attackMode = false;
        detectRadius = 10f;
        ExclamationPoint.SetActive(false);
        canAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = radiusOrigin == null ? Vector3.zero : radiusOrigin.position;
        Gizmos.DrawWireSphere(position, moveRadius);
        Gizmos.color = Color.red;
        Vector3 positionDetect = detectOrigin == null ? Vector3.zero : detectOrigin.position;
        Gizmos.DrawWireSphere(positionDetect, detectRadius);
    }
}

