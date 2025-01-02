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

            if (pickupSound != null)
            {
                AudioManager.Instance.PlaySound(pickupSound, transform.position);
            }

            animator.SetTrigger("PickUp");

            bool pickedUp = false;
            if (!pickedUp)
            {
                pickedUp = true;
                PlayerLoot.IncreaseItem(itemName, 1);
            }
            
            Debug.Log(PlayerLoot.GetItemAmount(itemName));
            Invoke("PickupComplete", 0.2f);
        }
    }

    private void PickupComplete()
    {
        Destroy(collectableItem);
    }
}
