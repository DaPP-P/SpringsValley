using System;
using TMPro;
using UnityEngine;

public enum VendorQuestState
{
    Available,
    Active,
    Completed
}

public class Quest : MonoBehaviour
{
    public const string RequiredCropOne = "corn";
    public const string RequiredCropTwo = "wheat";
    public const int RequiredCropAmount = 2;
    public const string RewardItem = "healing_potion";
    public const int RewardAmount = 2;

    public TextMeshProUGUI bodyText;

    public static VendorQuestState VendorState { get; private set; } = VendorQuestState.Available;
    public static bool vendorStarterQuestActive => VendorState == VendorQuestState.Active;
    public static bool vendorStarterQuestCompleted => VendorState == VendorQuestState.Completed;

    public static event Action VendorQuestChanged;

    public static string ActiveQuestText
    {
        get
        {
            switch (VendorState)
            {
                case VendorQuestState.Active:
                    return "Vendor Quest: Bring the vendor 2 corn and 2 wheat. Reward: 2 healing potions.";
                case VendorQuestState.Completed:
                    return "Vendor Quest Completed. Reward: 2 healing potions.";
                default:
                    return "No Active Quests";
            }
        }
    }

    private void OnEnable()
    {
        VendorQuestChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        VendorQuestChanged -= Refresh;
    }

    public static bool TryAcceptVendorStarterQuest()
    {
        if (VendorState != VendorQuestState.Available)
        {
            return false;
        }

        VendorState = VendorQuestState.Active;
        VendorQuestChanged?.Invoke();
        return true;
    }

    public static bool TryCompleteVendorStarterQuest()
    {
        if (VendorState != VendorQuestState.Active ||
            PlayerLoot.GetItemAmount(RequiredCropOne) < RequiredCropAmount ||
            PlayerLoot.GetItemAmount(RequiredCropTwo) < RequiredCropAmount)
        {
            return false;
        }

        if (!PlayerLoot.RemoveItem(RequiredCropOne, RequiredCropAmount))
        {
            return false;
        }

        if (!PlayerLoot.RemoveItem(RequiredCropTwo, RequiredCropAmount))
        {
            PlayerLoot.IncreaseItem(RequiredCropOne, RequiredCropAmount);
            return false;
        }

        PlayerLoot.IncreaseItem(RewardItem, RewardAmount);
        VendorState = VendorQuestState.Completed;
        VendorQuestChanged?.Invoke();
        return true;
    }

    public static void ResetAllQuests()
    {
        VendorState = VendorQuestState.Available;
        VendorQuestChanged?.Invoke();
    }

    private void Refresh()
    {
        if (bodyText != null)
        {
            bodyText.text = ActiveQuestText;
        }
    }
}
