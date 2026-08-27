using System.Collections.Generic;
using UnityEngine;

public class MerchantScript : MonoBehaviour
{
    public GameObject shopIcon;
    public GameObject questIcon;
    public GameObject interactionZone;

    private readonly HashSet<Collider2D> playerColliders = new HashSet<Collider2D>();
    private VendorLoot vendorLoot;

    private void Awake()
    {
        vendorLoot = GetComponentInChildren<VendorLoot>(true);
        SetIconVisibility(false);
    }

    private void OnEnable()
    {
        Quest.VendorQuestChanged += RefreshIcons;
        if (vendorLoot != null)
        {
            vendorLoot.StockChanged += RefreshIcons;
        }
    }

    private void OnDisable()
    {
        Quest.VendorQuestChanged -= RefreshIcons;
        if (vendorLoot != null)
        {
            vendorLoot.StockChanged -= RefreshIcons;
        }

        playerColliders.Clear();
        SetIconVisibility(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerColliders.Add(other);
            RefreshIcons();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerColliders.Remove(other);
            RefreshIcons();
        }
    }

    private void RefreshIcons()
    {
        SetIconVisibility(playerColliders.Count > 0);
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
}
