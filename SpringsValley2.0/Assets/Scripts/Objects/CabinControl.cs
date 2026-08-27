using System.Collections.Generic;
using UnityEngine;

public class CabinControl : MonoBehaviour
{
    public Animator doorAnimator;
    public Animator roofAnimator;

    [Header("Cabin Chest")]
    [SerializeField] private bool configureCabinChest = true;
    [SerializeField] private string cabinChestRewardItem = "coin";
    [SerializeField, Min(1)] private int cabinChestRewardAmount = 1;
    [SerializeField] private string cabinChestOpeningState = "cabin chest opening";
    [SerializeField, Min(0.1f)] private float cabinChestTriggerRadius = 0.45f;

    private readonly HashSet<Collider2D> playerColliders = new HashSet<Collider2D>();
    private bool cabinOpen;

    private void Awake()
    {
        if (configureCabinChest)
        {
            ConfigureCabinChest();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayerCollider(other))
        {
            return;
        }

        playerColliders.Add(other);
        SetOpen(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayerCollider(other))
        {
            return;
        }

        playerColliders.Remove(other);
        SetOpen(playerColliders.Count > 0);
    }

    private void OnDisable()
    {
        playerColliders.Clear();
        SetOpen(false);
    }

    private void LateUpdate()
    {
        ApplyOpenState();
    }

    private void SetOpen(bool isOpen)
    {
        cabinOpen = isOpen;
        ApplyOpenState();
    }

    private void ApplyOpenState()
    {
        if (CanSetAnimatorParameter(doorAnimator))
        {
            doorAnimator.SetBool("open", cabinOpen);
        }

        if (CanSetAnimatorParameter(roofAnimator))
        {
            roofAnimator.SetBool("open", cabinOpen);
        }
    }

    private static bool CanSetAnimatorParameter(Animator targetAnimator)
    {
        return targetAnimator != null && targetAnimator.isActiveAndEnabled && targetAnimator.isInitialized &&
            targetAnimator.runtimeAnimatorController != null;
    }

    private static bool IsPlayerCollider(Collider2D other)
    {
        return other != null &&
            (other.CompareTag("Player") || other.transform.root.CompareTag("Player"));
    }

    private void ConfigureCabinChest()
    {
        Transform chestTransform = null;
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "CabinChest")
            {
                chestTransform = child;
                break;
            }
        }

        if (chestTransform == null)
        {
            Debug.LogWarning("CabinControl could not find a child named CabinChest.", this);
            return;
        }

        bool hasTriggerCollider = false;
        foreach (Collider2D chestCollider in chestTransform.GetComponents<Collider2D>())
        {
            if (chestCollider.isTrigger)
            {
                hasTriggerCollider = true;
                break;
            }
        }

        if (!hasTriggerCollider)
        {
            CircleCollider2D chestTrigger = chestTransform.gameObject.AddComponent<CircleCollider2D>();
            chestTrigger.isTrigger = true;
            chestTrigger.radius = cabinChestTriggerRadius;
        }

        InteractionTrigger interaction = chestTransform.GetComponent<InteractionTrigger>();
        if (interaction == null)
        {
            interaction = chestTransform.gameObject.AddComponent<InteractionTrigger>();
        }

        interaction.ConfigureInventoryReward(
            chestTransform.GetComponent<Animator>(),
            cabinChestRewardItem,
            cabinChestRewardAmount,
            cabinChestOpeningState);
    }
}
