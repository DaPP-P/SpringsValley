  using System.Linq.Expressions;
using UnityEngine;

public class VendorScript : MonoBehaviour
{
    private bool possibleQuests;
    private bool availableItems;

    public GameObject shopIcon;
    public GameObject questIcon;
    public GameObject interactionZone;
    private BoxCollider2D interactionCollider;
    private Transform playerTransform;

    public TextBox currentTextBox;

    private bool VendorTextOpen;

    public GameObject vendorshopUI;
    public GameObject vendorquestUI;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

    // Update is called once per frame
    void Update()
{
    if (playerTransform == null) return;

    // Checks if player can interact with the vendor
    if (interactionCollider.OverlapPoint(playerTransform.position) && !VendorTextOpen)
    {
        Vector3 textBoxPosition = transform.position + new Vector3(0, 0.425f, 0);  
        currentTextBox = TextBox.Create(textBoxPosition, "Greetings, wanna <color=black>shop</color> or view <color=black>quests</color>?");
        VendorTextOpen = true;
        shopIcon.SetActive(false);
        questIcon.SetActive(false);
    } 
    else if (!interactionCollider.OverlapPoint(playerTransform.position) && currentTextBox != null)
    {
        VendorTextOpen = false;
        Destroy(currentTextBox.gameObject);
        shopIcon.SetActive(true);
        questIcon.SetActive(true);
    }

    // Handle input for selection
    if (currentTextBox != null)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentTextBox.SelectedVendorOptions(vendorshopUI, vendorquestUI);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) currentTextBox.ScrollOptions(-1); // Scroll up
        if (scroll < 0f) currentTextBox.ScrollOptions(1);  // Scroll down

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            vendorshopUI.SetActive(false);
            vendorquestUI.SetActive(false);
   
        }
    }
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
