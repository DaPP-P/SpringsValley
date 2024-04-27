using UnityEngine;

public class FollowPlayerWithDelay : MonoBehaviour
{
    public Transform playerTransform;
    public float followDelay = 0.5f; // Adjust the delay as needed
    private Vector3 offset;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not set!");
            enabled = false; // Disable script if playerTransform is not set
            return;
        }

        offset = transform.position - playerTransform.position;
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        Vector3 targetPosition = playerTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followDelay * Time.deltaTime);
    }
}
