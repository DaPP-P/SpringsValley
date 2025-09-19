using System.Linq.Expressions;
using UnityEngine;

public class MerchantScript : MonoBehaviour
{

    private bool possibleQuests;
    private bool availableItems;
    public GameObject shopIcon;
    public GameObject questIcon;
    public GameObject interactionZone;
    private BoxCollider2D interactionCollider;
    private bool VendorTextOpen;
    private Transform playerTransform;


    void Start()
    {

        availableItems = true;


        if (Quests.VendorStarterQuest)
        {
            possibleQuests = true;
        }
        else 
        {
            possibleQuests = false;
        }

        availableItems = true;
        questIcon.SetActive(false);
        shopIcon.SetActive(false);
        VendorTextOpen = false;

        interactionCollider = interactionZone.GetComponent<BoxCollider2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("Found player object");
        } else
        {
            Debug.Log("cant find Player object");
        }
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player inside");

            if (availableItems)
            {
                Debug.Log("Player can access shop");
                shopIcon.SetActive(true);
            } else {
                shopIcon.SetActive(false);
                Debug.Log("No items available");
            }

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