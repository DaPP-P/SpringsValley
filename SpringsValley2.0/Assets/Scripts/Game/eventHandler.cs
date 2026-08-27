using UnityEngine;

public class eventHandler : MonoBehaviour
{
    public GameObject targetObject;
    private PlayerMovement targetScript;

    private void Awake()
    {
        if (targetObject != null)
        {
            targetScript = targetObject.GetComponent<PlayerMovement>();
        }

        if (targetScript == null)
        {
            targetScript = GetComponentInParent<PlayerMovement>();
        }
    }

    // Called by the attack animation event when one is configured.
    public void OnAnimationEnd()
    {
        if (targetScript != null)
        {
            targetScript.OnAttackAnimationComplete();
        }
    }
}
