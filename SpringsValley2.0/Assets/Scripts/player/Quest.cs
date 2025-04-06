using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quest : MonoBehaviour
{

    public TextMeshProUGUI bodyText;
    public static bool vendorStarterQuestActive = false;

    // List of Quests
    public static bool vendorStarterQuestCompleted = false;

    void Start()
    {
        bodyText.text = "No Active Quests";
    }

    // Update is called once per frame
    void Update()
    {
        if (vendorStarterQuestActive) {
            bodyText.text = "Quest from the Vendor: Please collect 2 x wheat & 2 x Hay. Needs to be return back to the vendor. Reward : 2 x speed potion.";
        }

        if (vendorStarterQuestCompleted) {
            bodyText.text = "Start Vendor Quest Completed. Reward: 2 speed potions";
        }
    
    }

}
