using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class EnemyHealthSystem : MonoBehaviour
{

    public int health;
    public int healthMax;
    public GameObject enemySprite;
    private SpriteRenderer characterSpriteRenderer;
    private Color spriteOriginalColor;
    public GameObject damageTextObject;
    private TextMeshPro damageText;
    private bool isTakingDamage = false;

    // Start is called before the first frame update
    void Start()
    {
        health = healthMax;

        characterSpriteRenderer = enemySprite.GetComponent<SpriteRenderer>();
        spriteOriginalColor = characterSpriteRenderer.color;
        damageText = damageTextObject.GetComponent<TextMeshPro>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("weapon") && !isTakingDamage)
        {
            // Assuming that the weapon deals a fixed amount of damage, adjust this as needed
            int damageAmount = playerControl.weaponDamage;
            Damage(damageAmount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("weapon"))
        {
            // Reset the flag when the weapon exits the trigger zone
            isTakingDamage = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth()
    {
        return health;
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        
        StartCoroutine(DamageIndication("-"+damageAmount, Color.red, 0.2f));
        
        if (health <= 0) {
            health = 0;
            StartCoroutine(DeleteSprite(0.2f));
        }
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > healthMax) health = healthMax;
    }

    public IEnumerator DamageIndication(string text, Color textColor, float duration) {

        characterSpriteRenderer.color = textColor;
        damageText.text = text;
        damageText.color = textColor;
        damageTextObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        damageTextObject.SetActive(false);
        characterSpriteRenderer.color = spriteOriginalColor;
    }

    private IEnumerator DeleteSprite(float duration) {
        yield return new WaitForSeconds(duration);
        Destroy(enemySprite);
    }


}

