using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class VendorQuests : MonoBehaviour
{

    public TextMeshProUGUI activeQuest;
    public TextMeshProUGUI newQuest;
    public Button newQuestBtn;

    void Start()
    {
        newQuest.gameObject.SetActive(true);
        activeQuest.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcceptQuestBtn()
    {
        newQuestBtn.gameObject.SetActive(false);
        newQuest.gameObject.SetActive(false);
        activeQuest.gameObject.SetActive(true);
        Quest.vendorStarterQuestActive = true;
    }
}
