using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public Animator animator;
    public GameObject icon;
    public GameObject prefabToSpawn;
    public Transform spawnLocation;

    [Header("Interaction")]
    [SerializeField, Min(0.25f)] private float interactionRange = 1.75f;

    [Header("Direct inventory reward")]
    [SerializeField] private string inventoryRewardItem;
    [SerializeField, Min(0)] private int inventoryRewardAmount;
    [SerializeField] private string openAnimationStateName = "cabin chest opening";
    [SerializeField] private bool requireTriggerContact;

    private readonly HashSet<Collider2D> playerColliders = new HashSet<Collider2D>();
    private Transform playerTransform;
    private bool chestOpen;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player != null ? player.transform : null;

        EnsureInteractionPrompt();
        SetIconVisible(false);
    }

    public void ConfigureInventoryReward(Animator chestAnimator, string itemName, int amount, string animationStateName)
    {
        if (chestAnimator != null)
        {
            animator = chestAnimator;
        }

        inventoryRewardItem = itemName;
        inventoryRewardAmount = Mathf.Max(0, amount);
        requireTriggerContact = true;
        if (!string.IsNullOrWhiteSpace(animationStateName))
        {
            openAnimationStateName = animationStateName;
        }

        PrepareClosedAnimationFrame();

        EnsureInteractionPrompt();
        SetIconVisible(false);
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player != null ? player.transform : null;
        }

        bool canInteract = IsPlayerNear() && !chestOpen && !UIManager.UIOpen;
        SetIconVisible(canInteract);

        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            playerColliders.Add(other);
            playerTransform = other.transform.root;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            playerColliders.Remove(other);
        }
    }

    private void OnDisable()
    {
        playerColliders.Clear();
        SetIconVisible(false);
    }

    private void OpenChest()
    {
        if (!HasConfiguredReward())
        {
            ItemNotificationUI.ShowEffect("Chest is empty");
            Debug.LogWarning("Chest cannot open without a valid reward assignment.", this);
            return;
        }

        chestOpen = true;
        SetIconVisible(false);
        ItemNotificationUI.ShowEffect("Chest opened");

        if (animator != null)
        {
            PlayOpenAnimation();
        }

        bool gaveInventoryReward = inventoryRewardAmount > 0 &&
            PlayerLoot.IncreaseItem(inventoryRewardItem, inventoryRewardAmount);

        if (prefabToSpawn != null)
        {
            Vector3 spawnPosition = spawnLocation != null ? spawnLocation.position : transform.position;
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
        else if (!gaveInventoryReward)
        {
            Debug.LogWarning("Chest opened without a valid reward assignment.", this);
        }
    }

    private void SetIconVisible(bool visible)
    {
        if (icon != null)
        {
            icon.SetActive(visible);
        }
    }

    private void EnsureInteractionPrompt()
    {
        if (icon != null)
        {
            return;
        }

        Transform existingPrompt = transform.Find("Interaction Prompt");
        if (existingPrompt != null)
        {
            icon = existingPrompt.gameObject;
            return;
        }

        GameObject promptObject = new GameObject("Interaction Prompt", typeof(RectTransform));
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        promptObject.transform.localScale = Vector3.one * 0.15f;

        TextMeshPro promptText = promptObject.AddComponent<TextMeshPro>();
        promptText.text = "[ E ]";
        promptText.fontSize = 5f;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = new Color(1f, 0.92f, 0.35f, 1f);
        promptText.outlineColor = Color.black;
        promptText.outlineWidth = 0.25f;
        promptText.textWrappingMode = TextWrappingModes.NoWrap;
        promptText.rectTransform.sizeDelta = new Vector2(6f, 2f);
        promptText.raycastTarget = false;

        MeshRenderer promptRenderer = promptObject.GetComponent<MeshRenderer>();
        if (promptRenderer != null)
        {
            promptRenderer.sortingOrder = 30000;
        }

        icon = promptObject;
    }

    private void PlayOpenAnimation()
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == "OpenChest")
            {
                animator.speed = 1f;
                animator.SetTrigger(parameter.nameHash);
                return;
            }
        }

        if (!string.IsNullOrWhiteSpace(openAnimationStateName))
        {
            animator.speed = 1f;
            animator.Play(openAnimationStateName, 0, 0f);
            StartCoroutine(StopFallbackAnimationAtLastFrame());
        }
    }

    private void PrepareClosedAnimationFrame()
    {
        if (animator == null || animator.runtimeAnimatorController == null ||
            string.IsNullOrWhiteSpace(openAnimationStateName))
        {
            return;
        }

        animator.Play(openAnimationStateName, 0, 0f);
        animator.Update(0f);
        animator.speed = 0f;
    }

    private System.Collections.IEnumerator StopFallbackAnimationAtLastFrame()
    {
        yield return null;

        while (animator != null && animator.isActiveAndEnabled && animator.speed > 0f &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.98f)
        {
            yield return null;
        }

        if (animator != null && animator.isActiveAndEnabled)
        {
            animator.Play(openAnimationStateName, 0, 0.999f);
            animator.Update(0f);
            animator.speed = 0f;
        }
    }

    private bool IsPlayerNear()
    {
        if (playerColliders.Count > 0)
        {
            return true;
        }

        if (requireTriggerContact)
        {
            return false;
        }

        return playerTransform != null &&
            ((Vector2)playerTransform.position - (Vector2)transform.position).sqrMagnitude <= interactionRange * interactionRange;
    }

    private bool HasConfiguredReward()
    {
        bool hasInventoryReward = inventoryRewardAmount > 0 &&
            !string.IsNullOrWhiteSpace(inventoryRewardItem) &&
            (inventoryRewardItem == "coin" || ItemList.items.ContainsKey(inventoryRewardItem));
        return hasInventoryReward || prefabToSpawn != null;
    }

    private static bool IsPlayerCollider(Collider2D other)
    {
        return other != null &&
            (other.CompareTag("Player") || other.transform.root.CompareTag("Player"));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
