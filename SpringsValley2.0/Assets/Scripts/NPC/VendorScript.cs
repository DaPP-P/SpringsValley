using UnityEngine;

public class VendorScript : MonoBehaviour
{
    private bool possibleQuests;
    private bool availableItems;

    public GameObject shopIcon;
    public GameObject questIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shopIcon.SetActive(false);
        questIcon.SetActive(false);
        possibleQuests = false;
        availableItems = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Add update logic here if necessary
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player inside");

            if (availableItems && possibleQuests)
            {
                // Adjust shopIcon position and show both icons
                shopIcon.transform.localPosition = new Vector3(0.1f, shopIcon.transform.localPosition.y, shopIcon.transform.localPosition.z);
                shopIcon.SetActive(true);
                questIcon.SetActive(true);
            }
            else if (availableItems && !possibleQuests)
            {
                // Adjust shopIcon position and show only the shop icon
                shopIcon.transform.localPosition = new Vector3(0f, shopIcon.transform.localPosition.y, shopIcon.transform.localPosition.z);
                shopIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the collider is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left");
            shopIcon.SetActive(false);
            questIcon.SetActive(false);
        }
    }
}
