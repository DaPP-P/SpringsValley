using UnityEngine;

public class CoinScript : MonoBehaviour
{

    public Animator animator;
    public AudioClip pickupSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

            animator.SetTrigger("PickUp");
            PlayerLoot.IncreaseItem("coin", 1);
            Debug.Log(PlayerLoot.GetItemAmount("coin"));
            Invoke("pickupComplete", 0.2f);
        }
    }

    private void pickupComplete()
    {
        Destroy(gameObject);
    }
}
