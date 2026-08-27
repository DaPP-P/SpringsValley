using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorQuests : MonoBehaviour
{
    public TextMeshProUGUI activeQuest;
    public TextMeshProUGUI newQuest;
    public Button newQuestBtn;
    public Button completeQuestBtn;

    private void OnEnable()
    {
        Quest.VendorQuestChanged += Refresh;
        PlayerLoot.InventoryChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        Quest.VendorQuestChanged -= Refresh;
        PlayerLoot.InventoryChanged -= Refresh;
    }

    public void AcceptQuestBtn()
    {
        if (Quest.TryAcceptVendorStarterQuest())
        {
            ItemNotificationUI.ShowEffect("Vendor quest accepted");
            PlaySound(true);
        }
        else
        {
            PlaySound(false);
        }
    }

    public void CompleteQuestBtn()
    {
        if (Quest.TryCompleteVendorStarterQuest())
        {
            ItemNotificationUI.ShowEffect("Vendor quest completed");
            PlaySound(true);
        }
        else
        {
            PlaySound(false);
        }
    }

    private void Refresh()
    {
        bool isAvailable = Quest.VendorState == VendorQuestState.Available;
        bool isActive = Quest.VendorState == VendorQuestState.Active;
        bool isCompleted = Quest.VendorState == VendorQuestState.Completed;

        if (newQuest != null)
        {
            newQuest.gameObject.SetActive(isAvailable);
            newQuest.text = "Starter Vendor Quest: Bring back 2 corn and 2 wheat. Reward: 2 healing potions.";
        }

        if (newQuestBtn != null)
        {
            newQuestBtn.gameObject.SetActive(isAvailable);
        }

        if (activeQuest != null)
        {
            activeQuest.gameObject.SetActive(isActive || isCompleted);
            activeQuest.text = isCompleted
                ? "Starter Vendor Quest completed. Reward: 2 healing potions."
                : "Collect 2 corn and 2 wheat, then return to the vendor.";
        }

        if (completeQuestBtn != null)
        {
            completeQuestBtn.gameObject.SetActive(isActive);
            completeQuestBtn.interactable = isActive &&
                PlayerLoot.GetItemAmount(Quest.RequiredCropOne) >= Quest.RequiredCropAmount &&
                PlayerLoot.GetItemAmount(Quest.RequiredCropTwo) >= Quest.RequiredCropAmount;
        }
    }

    private void PlaySound(bool accepted)
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioClip clip = accepted ? AudioManager.Instance.acceptSound : AudioManager.Instance.declineSound;
        AudioManager.Instance.PlaySound(clip, transform.position);
    }
}
