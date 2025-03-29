using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class VendorQuests : MonoBehaviour
{

    public TextMeshProUGUI activeQuest;
    public TextMeshProUGUI newQuest;
    public Button newQuestBtn;
    public Button completeQuestBtn;

    void Start()
    {
        newQuest.gameObject.SetActive(true);
        completeQuestBtn.gameObject.SetActive(false);
        activeQuest.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
       if (Quest.vendorStarterQuestActive) {
            completeQuestBtn.gameObject.SetActive(true);
        } else {
            completeQuestBtn.gameObject.SetActive(false);
       } 
    }

    public void AcceptQuestBtn()
    {
        newQuestBtn.gameObject.SetActive(false);
        newQuest.gameObject.SetActive(false);
        activeQuest.gameObject.SetActive(true);
        Quest.vendorStarterQuestActive = true;
    }

    public void CompleteQuestBtn()
    {
        if ((PlayerLoot.GetItemAmount("corn") > 1) && PlayerLoot.GetItemAmount("wheat") > 1) {
            PlayerLoot.RemoveItem("corn", 2);
            PlayerLoot.RemoveItem("wheat", 2);
            PlayerLoot.IncreaseItem("healing_potion", 2);
            Quest.vendorStarterQuestActive = false;
            Quest.vendorStarterQuestCompleted = true;
            activeQuest.text = "Start Vendor Quest Completed. Reward: 2 speed potions";
        }
    }
}
