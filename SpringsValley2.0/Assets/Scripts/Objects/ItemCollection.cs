using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    public GameObject collectableItem;
    public string itemName;
    private bool canBeCollected;

    // Audio settings
    public AudioClip pickupSound;
    public Animator animator;

    void Start()
    {
        canBeCollected = false;
        animator = GetComponent<Animator>();
        Invoke("EnableCollection", 0.5f);
    }

    private void EnableCollection()
    {
        canBeCollected = true;
        Debug.Log("can be colelceted");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!canBeCollected) return;

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
