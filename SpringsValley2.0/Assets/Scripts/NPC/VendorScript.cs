using UnityEngine;

public class VendorScript : MonoBehaviour
{
    public GameObject shopIcon;
    public GameObject questIcon;
    public GameObject interactionZone;
    public TextBox currentTextBox;
    public GameObject vendorshopUI;
    public GameObject vendorquestUI;

    private BoxCollider2D interactionCollider;
    private Transform playerTransform;
    private VendorLoot vendorLoot;
    private bool vendorTextOpen;

    private void OnEnable()
    {
        Quest.VendorQuestChanged += RefreshIcons;
    }

    private void Start()
    {
        interactionCollider = interactionZone != null ? interactionZone.GetComponent<BoxCollider2D>() : null;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player != null ? player.transform : null;
        vendorLoot = GetComponentInChildren<VendorLoot>(true);

        if (vendorLoot != null)
        {
            vendorLoot.StockChanged += RefreshIcons;
        }

        SetIconVisibility(false);

        if (interactionCollider == null)
        {
            Debug.LogWarning("VendorScript needs an interactionZone with a BoxCollider2D.", this);
        }
        else if (playerTransform == null)
        {
            Debug.LogWarning("VendorScript could not find an object tagged Player.", this);
        }
    }

    private void OnDisable()
    {
        Quest.VendorQuestChanged -= RefreshIcons;
        if (vendorLoot != null)
        {
            vendorLoot.StockChanged -= RefreshIcons;
        }

        CloseTextBox();
        SetIconVisibility(false);
    }

    private void Update()
    {
        if (playerTransform == null || interactionCollider == null)
        {
            return;
        }

        bool playerInRange = interactionCollider.OverlapPoint(playerTransform.position);
        if (!playerInRange)
        {
            CloseTextBox();
            SetIconVisibility(false);
            return;
        }

        if (!vendorTextOpen && !UIManager.UIOpen)
        {
            Vector3 textBoxPosition = transform.position + new Vector3(0f, 2.125f, 0f);
            currentTextBox = TextBox.Create(
                textBoxPosition,
                "Greetings, want to <color=black>shop</color> or view <color=black>quests</color>?");
            vendorTextOpen = currentTextBox != null;
        }

        SetIconVisibility(!vendorTextOpen && !UIManager.UIOpen);

        if (currentTextBox == null || UIManager.UIOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentTextBox.SelectedVendorOptions(vendorshopUI, vendorquestUI);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            currentTextBox.ScrollOptions(-1);
        }
        else if (scroll < 0f)
        {
            currentTextBox.ScrollOptions(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RefreshIcons();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        CloseTextBox();
        SetIconVisibility(false);
    }

    private void RefreshIcons()
    {
        bool inRange = playerTransform != null && interactionCollider != null &&
            interactionCollider.OverlapPoint(playerTransform.position);
        SetIconVisibility(inRange && !vendorTextOpen && !UIManager.UIOpen);
    }

    private void SetIconVisibility(bool show)
    {
        bool hasItems = vendorLoot == null || vendorLoot.HasStock();
        bool hasQuest = Quest.VendorState != VendorQuestState.Completed;

        if (shopIcon != null)
        {
            shopIcon.SetActive(show && hasItems);
            Vector3 localPosition = shopIcon.transform.localPosition;
            localPosition.x = show && hasItems && hasQuest ? 0.1f : 0f;
            shopIcon.transform.localPosition = localPosition;
        }

        if (questIcon != null)
        {
            questIcon.SetActive(show && hasQuest);
        }
    }

    private void CloseTextBox()
    {
        vendorTextOpen = false;
        if (currentTextBox != null)
        {
            Destroy(currentTextBox.gameObject);
            currentTextBox = null;
        }
    }

}
