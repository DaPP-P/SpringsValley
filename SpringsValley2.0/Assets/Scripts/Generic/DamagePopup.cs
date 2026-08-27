using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private const float DisappearTimerMax = 1f;
    private const int SortingOrderBase = 10000;

    private static int sortingOrder;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        if (GameAssets.i == null || GameAssets.i.pfDamagePopup == null)
        {
            return null;
        }

        Transform popupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
        DamagePopup popup = popupTransform.GetComponent<DamagePopup>();
        if (popup == null)
        {
            Debug.LogError("The configured damage popup prefab has no DamagePopup component.", popupTransform);
            Destroy(popupTransform.gameObject);
            return null;
        }

        popup.Setup(damageAmount);
        return popup;
    }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount)
    {
        if (textMesh == null)
        {
            Destroy(gameObject);
            return;
        }

        textMesh.SetText(Mathf.Max(0, damageAmount).ToString());
        textColor = textMesh.color;
        disappearTimer = DisappearTimerMax;

        sortingOrder = (sortingOrder + 1) % 1000;
        textMesh.sortingOrder = SortingOrderBase + sortingOrder;
        moveVector = new Vector3(6f, 12f, 0f);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        transform.position += moveVector * deltaTime;
        moveVector = Vector3.Lerp(moveVector, Vector3.zero, 8f * deltaTime);

        disappearTimer -= deltaTime;
        if (disappearTimer > DisappearTimerMax * 0.5f)
        {
            transform.localScale += Vector3.one * deltaTime;
        }
        else if (disappearTimer > 0f)
        {
            transform.localScale = Vector3.Max(Vector3.zero, transform.localScale - Vector3.one * deltaTime);
        }
        else
        {
            textColor.a -= 1.5f * deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
