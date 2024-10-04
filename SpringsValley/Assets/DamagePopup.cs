using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disapperTimer;
    private Color textColor;

    private const float DISAPPEAR_TIMER_MAX = 1f;
    private Vector3 moveVector;
    private static int sortingOrder;

    public static DamagePopup Create(Vector3 position, int damageAmount) {
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);
    
    return damagePopup;
    }

    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount) {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disapperTimer = DISAPPEAR_TIMER_MAX;
        
        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
        moveVector = new Vector3(.1f, .2f) * 60f;
    }

    private void Update() {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disapperTimer > DISAPPEAR_TIMER_MAX * 0.5f) {
            // First half of the popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }
        disapperTimer -= Time.deltaTime;
        if (disapperTimer < 0) {
            float disappearSpeed = 1.5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0) {
                Destroy(gameObject);
            }
        }
    }
}
