using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    public GameObject collectableItem;
    public string itemName;
    public AudioClip pickupSound;
    public Animator animator;

    [SerializeField, Min(0f)] private float collectionDelay = 0.5f;
    [SerializeField, Min(0f)] private float pickupAnimationDuration = 0.2f;

    private bool canBeCollected;
    private bool pickedUp;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        Invoke(nameof(EnableCollection), collectionDelay);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBeCollected || pickedUp || !other.CompareTag("Player"))
        {
            return;
        }

        if (!PlayerLoot.IncreaseItem(itemName, 1))
        {
            Debug.LogWarning($"Could not collect item with invalid name '{itemName}'.", this);
            return;
        }

        pickedUp = true;
        canBeCollected = false;
        DisableColliders();

        if (AudioManager.Instance != null && pickupSound != null)
        {
            AudioManager.Instance.PlaySound(pickupSound, transform.position);
        }

        if (animator != null)
        {
            animator.SetTrigger("PickUp");
        }

        Invoke(nameof(PickupComplete), pickupAnimationDuration);
    }

    private void EnableCollection()
    {
        canBeCollected = !pickedUp;
    }

    private void DisableColliders()
    {
        foreach (Collider2D itemCollider in GetComponentsInChildren<Collider2D>())
        {
            itemCollider.enabled = false;
        }
    }

    private void PickupComplete()
    {
        Destroy(collectableItem != null ? collectableItem : gameObject);
    }
}
