using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class TextBox : MonoBehaviour
{
    private TextMeshPro textMesh;
    private static int sortingOrder;
    private string fullText;
    private string currentText = "";
    private float typingSpeed = 0.05f; // Time between each character

    private List<int> optionStartIndexes = new List<int>(); // Stores option start indexes
    private int selectedOption = 0; // Track which option is selected
    private bool textFullyDisplayed = false;

    public static TextBox Create(Vector3 position, string message)
    {
        Transform textBoxTransform = Instantiate(GameAssets.i.pfTextBox, position, Quaternion.identity);
        TextBox textBox = textBoxTransform.GetComponent<TextBox>();
        textBox.Setup(message);
        return textBox;
    }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(string message)
    {
        fullText = message;  // Store the full text
        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
        StartCoroutine(DisplayText());  // Start displaying text one character at a time
    }

    private IEnumerator DisplayText()
    {
        currentText = "";  // Reset the displayed text
        int visibleCharCount = 0; // Tracks how many visible characters are displayed
        bool insideTag = false;  // Track if we're inside a tag
        
        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];

            if (c == '<') insideTag = true;  // Start of a rich text tag
            if (!insideTag) visibleCharCount++;  // Only count visible characters
            if (c == '>') insideTag = false;  // End of a rich text tag

            currentText += c;  // Add the character (including tags)

            if (!insideTag)  // Only update visible text when not inside a tag
            {
                textMesh.SetText(currentText);
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        textFullyDisplayed = true; // Mark text as fully displayed
        ExtractOptions(); // Find selectable options
        UpdateHighlightedText(); // Highlight first option
    }

    private void ExtractOptions()
    {
        optionStartIndexes.Clear();
        int index = fullText.IndexOf("<color=black>");
    while (index != -1)
    {
        optionStartIndexes.Add(index);
        index = fullText.IndexOf("<color=black>", index + 1, System.StringComparison.Ordinal);
        }
    }

private void UpdateHighlightedText()
{
    string modifiedText = fullText;
    for (int i = 0; i < optionStartIndexes.Count; i++)
    {
        if (i == selectedOption)
        {
            modifiedText = modifiedText.Substring(0, optionStartIndexes[i]) + "<color=yellow>" +
            modifiedText.Substring(optionStartIndexes[i] + "<color=black>".Length);
        }
        else
        {
            modifiedText = modifiedText.Substring(0, optionStartIndexes[i]) + "<color=black>" +
            modifiedText.Substring(optionStartIndexes[i] + "<color=black>".Length);
        }
    }

    textMesh.SetText(modifiedText);
}


    public void ScrollOptions(int direction)
    {
        if (!textFullyDisplayed) return; // Don't scroll until text is fully displayed

        selectedOption += direction;
        if (selectedOption < 0) selectedOption = optionStartIndexes.Count - 1;
        if (selectedOption >= optionStartIndexes.Count) selectedOption = 0;

        UpdateHighlightedText();
    }

    public void SelectedVendorOptions(GameObject Vendorshop, GameObject questShop)
    {
        if (!textFullyDisplayed) return;
        
        int shopOption = 0;
        int questOption = 1;

        if (selectedOption == shopOption) {
            Debug.Log("shop option selected");
            Vendorshop.SetActive(true);
        } else if (selectedOption == questOption) {
            Debug.Log("quest option selected");
            questShop.SetActive(true);
        }
    }

}
