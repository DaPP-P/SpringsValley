using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target; // The object to follow
    public float followDelay = 0.5f; // The delay in seconds

    private Vector3 targetPosition;
    private Vector3 lastTargetPosition;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        // Initialize the target position
        targetPosition = target.position;
        lastTargetPosition = targetPosition;
    }

    private void Update()
    {
        // Update the target position
        targetPosition = target.position;

        // Move towards the target position with delay
        transform.position = Vector3.SmoothDamp(transform.position, lastTargetPosition, ref velocity, followDelay);

        // Update the last target position after the delay
        if (Time.time >= followDelay)
        {
            lastTargetPosition = targetPosition;
        }
    }
}