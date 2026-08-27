using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ItemNotificationUI : MonoBehaviour
{
    private static readonly Color ItemColour = new Color(0.18f, 0.65f, 0.3f, 0.96f);
    private static readonly Color EffectColour = new Color(0.2f, 0.48f, 0.78f, 0.96f);

    private static ItemNotificationUI instance;

    private RectTransform notificationContainer;

    public static void ShowItemAdded(string itemName, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        string displayName = itemName == "coin"
            ? amount == 1 ? "Coin" : "Coins"
            : ItemList.items.TryGetValue(itemName, out Item item) ? item.Name : itemName;

        Show($"+{amount} {displayName}", ItemColour);
    }

    public static void ShowEffect(string message)
    {
        Show(message, EffectColour);
    }

    private static void Show(string message, Color colour)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        ItemNotificationUI notificationUI = GetOrCreateInstance();
        if (notificationUI != null)
        {
            notificationUI.CreateNotification(message, colour);
        }
    }

    private static ItemNotificationUI GetOrCreateInstance()
    {
        if (instance != null)
        {
            return instance;
        }

        GameObject canvasObject = new GameObject("Item Notification UI", typeof(RectTransform));
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30000;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        instance = canvasObject.AddComponent<ItemNotificationUI>();
        instance.BuildContainer();
        return instance;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void BuildContainer()
    {
        GameObject containerObject = new GameObject("Notifications", typeof(RectTransform));
        containerObject.transform.SetParent(transform, false);
        notificationContainer = containerObject.GetComponent<RectTransform>();
        notificationContainer.anchorMin = new Vector2(0.5f, 1f);
        notificationContainer.anchorMax = new Vector2(0.5f, 1f);
        notificationContainer.pivot = new Vector2(0.5f, 1f);
        notificationContainer.anchoredPosition = new Vector2(0f, -35f);
        notificationContainer.sizeDelta = new Vector2(420f, 0f);

        VerticalLayoutGroup layout = containerObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = containerObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void CreateNotification(string message, Color colour)
    {
        if (notificationContainer == null)
        {
            BuildContainer();
        }

        GameObject notificationObject = new GameObject("Notification", typeof(RectTransform));
        notificationObject.transform.SetParent(notificationContainer, false);

        Image background = notificationObject.AddComponent<Image>();
        background.color = colour;
        background.raycastTarget = false;

        LayoutElement layoutElement = notificationObject.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 400f;
        layoutElement.preferredHeight = 48f;

        CanvasGroup canvasGroup = notificationObject.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        GameObject textObject = new GameObject("Text", typeof(RectTransform));
        textObject.transform.SetParent(notificationObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(16f, 4f);
        textRect.offsetMax = new Vector2(-16f, -4f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 22f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        if (TMP_Settings.defaultFontAsset != null)
        {
            text.font = TMP_Settings.defaultFontAsset;
        }

        StartCoroutine(NotificationLifetime(notificationObject, canvasGroup));
    }

    private static IEnumerator NotificationLifetime(GameObject notification, CanvasGroup canvasGroup)
    {
        yield return new WaitForSecondsRealtime(1.75f);

        const float fadeDuration = 0.3f;
        float elapsed = 0f;
        while (elapsed < fadeDuration && notification != null)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        if (notification != null)
        {
            Destroy(notification);
        }
    }
}
