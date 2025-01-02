using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    public GameObject collectableItem;
    public string itemName;

    // Audio settings
    public AudioClip pickupSound;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Play the pickup sound using AudioManager
            if (pickupSound != null)
            {
                AudioManager.Instance.PlaySound(pickupSound, transform.position);
            }

            // Increase item count and log it
            animator.SetTrigger("PickUp");

            bool pickedUp = false;
            if (!pickedUp) {
                pickedUp = true;
                PlayerLoot.IncreaseItem(itemName, 1);
            }
            Debug.Log(PlayerLoot.GetItemAmount(itemName));

            // Destroy the collectable item after a short delay
            Invoke("PickupComplete", 0.2f);
        }
    }

    private void PickupComplete()
    {
        Destroy(collectableItem);
    }
}
