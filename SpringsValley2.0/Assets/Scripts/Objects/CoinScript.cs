using UnityEngine;

public class CoinScript : MonoBehaviour
{

    public Animator animator;
    public AudioClip pickupSound;

    private bool canBeCollected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        Invoke("EnableCollection", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EnableCollection()
    {
        canBeCollected = true;
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
            if(!pickedUp)
            {
                pickedUp = true;
                PlayerLoot.IncreaseItem("coin", 1);
            }

            Debug.Log(PlayerLoot.GetItemAmount("coin"));
            Invoke("pickupComplete", 0.2f);
        }
    }

    private void pickupComplete()
    {
        Destroy(gameObject);
    }
}
