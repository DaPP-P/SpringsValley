using UnityEngine;

public class ItemCollection : MonoBehaviour
{

    public GameObject collectableItem;
    public string itemName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLoot.IncreaseItem(itemName, 1);
            Debug.Log(PlayerLoot.GetItemAmount(itemName));
            Invoke("pickupComplete", 0.2f); // Reset attack state after delay.
        }
    }

    private void pickupComplete()
    {
        Destroy(collectableItem);
    }
}
