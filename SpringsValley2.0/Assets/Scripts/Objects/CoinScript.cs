using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public Animator animator;
    public AudioClip pickupSound;

    [SerializeField, Min(0f)] private float collectionDelay = 0.2f;
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

        pickedUp = true;
        canBeCollected = false;
        PlayerLoot.AddCoins(1);

        foreach (Collider2D coinCollider in GetComponentsInChildren<Collider2D>())
        {
            coinCollider.enabled = false;
        }

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

    private void PickupComplete()
    {
        Destroy(gameObject);
    }
}
