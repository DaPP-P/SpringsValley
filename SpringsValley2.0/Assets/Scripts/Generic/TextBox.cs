using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    private const string OptionTag = "<color=black>";
    private const string SelectedOptionTag = "<color=yellow>";

    private static int sortingOrder;

    [SerializeField, Min(0f)] private float typingSpeed = 0.05f;

    private readonly List<int> optionStartIndexes = new List<int>();
    private TextMeshPro textMesh;
    private string fullText = string.Empty;
    private int selectedOption;
    private bool textFullyDisplayed;

    public static TextBox Create(Vector3 position, string message)
    {
        if (GameAssets.i == null || GameAssets.i.pfTextBox == null)
        {
            Debug.LogWarning("A TextBox was requested, but GameAssets has no TextBox prefab assigned.");
            return null;
        }

        Transform textBoxTransform = Instantiate(GameAssets.i.pfTextBox, position, Quaternion.identity);
        TextBox textBox = textBoxTransform.GetComponent<TextBox>();
        if (textBox == null)
        {
            Debug.LogError("The configured TextBox prefab does not contain a TextBox component.", textBoxTransform);
            Destroy(textBoxTransform.gameObject);
            return null;
        }

        textBox.Setup(message);
        return textBox;
    }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(string message)
    {
        fullText = message ?? string.Empty;
        selectedOption = 0;
        textFullyDisplayed = false;
        sortingOrder++;

        if (textMesh == null)
        {
            Debug.LogError("TextBox needs a TextMeshPro component.", this);
            return;
        }

        textMesh.sortingOrder = sortingOrder;
        StartCoroutine(DisplayText());
    }

    public void ScrollOptions(int direction)
    {
        if (!textFullyDisplayed || optionStartIndexes.Count == 0 || direction == 0)
        {
            return;
        }

        selectedOption = (selectedOption + direction) % optionStartIndexes.Count;
        if (selectedOption < 0)
        {
            selectedOption += optionStartIndexes.Count;
        }

        UpdateHighlightedText();
    }

    public void SelectedVendorOptions(GameObject vendorShop, GameObject questShop)
    {
        if (!textFullyDisplayed || optionStartIndexes.Count == 0)
        {
            return;
        }

        GameObject configuredUI = selectedOption == 0 ? vendorShop : selectedOption == 1 ? questShop : null;
        if (configuredUI != null)
        {
            // The prefab references its Canvas, while the inventory/quest controller lives on
            // a parent that may be inactive. Enable the Canvas locally, then open that parent.
            configuredUI.SetActive(true);
            GameObject uiRoot = configuredUI;

            if (selectedOption == 0)
            {
                VendorInventory inventory = configuredUI.GetComponentInParent<VendorInventory>(true);
                if (inventory != null)
                {
                    uiRoot = inventory.gameObject;
                }
            }
            else
            {
                VendorQuests quests = configuredUI.GetComponentInParent<VendorQuests>(true);
                if (quests != null)
                {
                    uiRoot = quests.gameObject;
                }
            }

            if (!UIManager.OpenUI(uiRoot))
            {
                configuredUI.SetActive(false);
            }
        }
    }

    private IEnumerator DisplayText()
    {
        string currentText = string.Empty;
        bool insideTag = false;

        for (int index = 0; index < fullText.Length; index++)
        {
            char character = fullText[index];
            if (character == '<')
            {
                insideTag = true;
            }

            currentText += character;

            if (character == '>')
            {
                insideTag = false;
            }

            if (!insideTag)
            {
                textMesh.SetText(currentText);
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        textFullyDisplayed = true;
        ExtractOptions();
        UpdateHighlightedText();
    }

    private void ExtractOptions()
    {
        optionStartIndexes.Clear();
        int searchStart = 0;

        while (searchStart < fullText.Length)
        {
            int optionIndex = fullText.IndexOf(OptionTag, searchStart, System.StringComparison.Ordinal);
            if (optionIndex < 0)
            {
                break;
            }

            optionStartIndexes.Add(optionIndex);
            searchStart = optionIndex + OptionTag.Length;
        }
    }

    private void UpdateHighlightedText()
    {
        if (textMesh == null)
        {
            return;
        }

        string modifiedText = fullText;
        for (int index = optionStartIndexes.Count - 1; index >= 0; index--)
        {
            int tagIndex = optionStartIndexes[index];
            string replacement = index == selectedOption ? SelectedOptionTag : OptionTag;
            modifiedText = modifiedText.Remove(tagIndex, OptionTag.Length).Insert(tagIndex, replacement);
        }

        textMesh.SetText(modifiedText);
    }
}
