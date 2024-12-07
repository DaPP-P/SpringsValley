using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    private bool isPlayerInside = false;
    public Animator animator;
    public GameObject icon;
    private bool chestOpen;

    // For spawning the coin
    public GameObject prefabToSpawn; // The prefab to spawn
    public Transform spawnLocation; // Optional: a specific location to spawn the prefab
    void Start()
    {
        icon.SetActive(false);
        chestOpen = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player inside");
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left");
            isPlayerInside = false;
        }
    }

    private void Update()
    {
        if (isPlayerInside && !chestOpen)
        {
            icon.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Player interact");
                animator.SetTrigger("OpenChest");
                chestOpen = true;

                // Spawn the prefab
                if (prefabToSpawn != null)
                {
                    Vector3 spawnPosition = spawnLocation != null
                        ? spawnLocation.position
                        : transform.position;

                    Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity); // Spawn the prefab
                    Debug.Log($"Prefab spawned at {spawnPosition}");
                }
                else
                {
                    Debug.Log("No Prefab assigned to spawn!");
                }
            }
        }
        else
        {
            icon.SetActive(false);
        }
    }
}
