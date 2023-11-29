using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameHandler : MonoBehaviour
{
    public Image healthBar;
    public GameObject mainCharacter;
    private SpriteRenderer characterSpriteRenderer;
    private HealthSystem healthSystem;
    private Color spriteOriginalColor;
    public GameObject damageTextObject;
    private TextMeshPro damageText;

    // Start is called before the first frame update
    void Start()
    {

        characterSpriteRenderer = mainCharacter.GetComponent<SpriteRenderer>();
        spriteOriginalColor = characterSpriteRenderer.color;

        healthSystem = new HealthSystem(100);
        healthSystem.OnTakeDamage += HandleTakeDamage;
        healthSystem.OnHeal += HandleHeal;

        damageText = damageTextObject.GetComponent<TextMeshPro>();

        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            healthSystem.Damage(20);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            healthSystem.Heal(20);
        }
    }

    void HandleTakeDamage(object sender, DamageEventArgs e)
    {
        int damageAmount = e.DamageAmount;


        UpdateHealthBar();
        StartCoroutine(showDamageText("-" + damageAmount, Color.red));
        StartCoroutine(FlashSpriteColor(Color.red, 0.2f));

    }

    void HandleHeal(object sender, HealEventArgs e)
    {
        int healAmount = e.HealAmount;

        StartCoroutine(showDamageText("+" + healAmount, Color.blue));
        StartCoroutine(FlashSpriteColor(Color.blue, 0.2f));
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = healthSystem.GetHealthPercentage();
    }

    public IEnumerator FlashSpriteColor(Color flashColor, float duration)
    {
        // Change the sprite color
        characterSpriteRenderer.color = flashColor;

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Restore the original color
        characterSpriteRenderer.color = spriteOriginalColor;
    }

    public IEnumerator showDamageText(string text, Color textColor) 
    {
        damageText.text = text;
        damageText.color = textColor;

        damageTextObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        damageTextObject.SetActive(false);
    }
}


