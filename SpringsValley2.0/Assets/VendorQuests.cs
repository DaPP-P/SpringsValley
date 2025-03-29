using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class VendorQuests : MonoBehaviour
{

    public TextMeshProUGUI activeQuest;
    public TextMeshProUGUI newQuest;
    public Button newQuestBtn;
    public Button completeQuestBtn;

    public bool newQuestCompletable;

    void Start()
    {
        newQuest.gameObject.SetActive(true);
        completeQuestBtn.gameObject.SetActive(false);
        activeQuest.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Quest.vendorStarterQuestActive);
       if (Quest.vendorStarterQuestActive) {
            if ((PlayerLoot.GetItemAmount("corn") > 1) && PlayerLoot.GetItemAmount("wheat") > 1)
            {
                newQuestCompletable = true;
                completeQuestBtn.gameObject.SetActive(true);
            }
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
        PlayerLoot.RemoveItem("corn", 2);
        PlayerLoot.RemoveItemItem("wheat", 2);
        PlayerLoot.IncreaseItem("healing_potion", 2);
    }
}
