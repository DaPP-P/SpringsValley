using UnityEngine;

public class eventHandler : MonoBehaviour
{
    public GameObject targetObject; // Assign the target GameObject in the Inspector
    private PlayerMovement targetScript;

    void Start()
    {
        // Ensure the target object has the correct script
        if (targetObject != null)
        {
            targetScript = targetObject.GetComponent<PlayerMovement>();
        }

    }

    // This method will be called by the Animation Event
    public void OnAnimationEnd()
    {
        if (targetScript != null)
        {
            targetScript.isAttacking = false; // Update the bool
            Debug.Log("Animation ended, updated target bool!");
        }
        else
        {
            Debug.LogError("Target Script is not found!");
        }
    }
}
