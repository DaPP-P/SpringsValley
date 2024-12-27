using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{

    public GameObject invslot1;
    private Image invimage1;
    public GameObject invslot2;
    private Image invimage2;

    public GameObject invslot3;
    private Image invimage3;

    public GameObject invslot4;
    private Image invimage4;

    [SerializeField] private SpriteRenderer cornRenderer;
    [SerializeField] private SpriteRenderer wheatRenderer;


    public TextMeshProUGUI coinText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (invslot1 != null)
        {
            invimage1 = invslot1.GetComponent<Image>();
        }
        if (invslot2 != null)
        {
            invimage2 = invslot2.GetComponent<Image>();
        }
        if (invslot3 != null)
        {
            invimage3 = invslot3.GetComponent<Image>();
        }
        if (invslot4 != null)
        {
            invimage4 = invslot4.GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = "Coins:" + PlayerLoot.GetItemAmount("coin").ToString();
        invimage1.sprite = cornRenderer.sprite; // Assign sprite to the Image
    }

}
