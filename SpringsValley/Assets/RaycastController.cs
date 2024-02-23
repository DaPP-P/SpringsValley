using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public Transform raycastOrigin; // The transform of the object performing the raycast
    public Transform targetObject; // The transform of the object being raycasted to
    public float raycastDistance = 10f; // Maximum distance of the raycast
    public float verticalOffset = 1f; // Distance above or below the center of the target object

    void Update()
    {
        // Original raycast from origin to center of the target object
        Vector3 originalDirection = targetObject.position - raycastOrigin.position;
        Debug.DrawRay(raycastOrigin.position, originalDirection, Color.green);

        // Modified raycast from origin to specified distance above or below the center of the target object
        Vector3 modifiedDirection = (targetObject.position + Vector3.up * verticalOffset) - raycastOrigin.position;
        Debug.DrawRay(raycastOrigin.position, modifiedDirection, Color.blue);

        // Perform the modified raycast
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.position, modifiedDirection, raycastDistance);

        // Check if the raycast hits the target object
        if (hit.collider != null && hit.collider.transform == targetObject)
        {
            // Handle the raycast hit
            Debug.Log("Raycast hit the target object!");
        }
    }
}
