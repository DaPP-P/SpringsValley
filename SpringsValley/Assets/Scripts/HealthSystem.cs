using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private int currentHealth, maxHealth;
    
    [SerializeField]
    private bool isDead = false;
    public GameObject damageTextObject;
    private TextMeshPro damageText;
    private SpriteRenderer characterSpriteRenderer;
    private Color spriteOriginalColor;
    public GameObject mainSprite;

    public float knockbackForce;
    public Rigidbody2D rb;

    public bool indicationTimeDone = false;
    
    //public event EventHandler<DamageEventArgs> OnTakeDamage;
    //public event EventHandler<HealEventArgs> OnHeal;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    void Start()
    {
        // Initialize the health system and find components needed for damage indications.
        InitializeHealthSystem(maxHealth);
        damageText = damageTextObject.GetComponent<TextMeshPro>();
        characterSpriteRenderer = mainSprite.GetComponent<SpriteRenderer>();
        spriteOriginalColor = characterSpriteRenderer.color;
        
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitializeHealthSystem(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;
    }

    public void Damage (int amount, GameObject sender) 
    {
        if (isDead)
            return;
        // So the sender can't find itself.
        if (sender.layer == gameObject.layer)
            return;

        
        currentHealth -= amount;

        StartCoroutine(DamageIndication("-"+amount, Color.red));   

        if (currentHealth > 0)
        {
             Debug.Log("Sender position: " + sender.transform.position);
             Knockback(knockbackForce, sender.transform.position);
        }
        else
        {
            isDead = true;
            Destroy(gameObject);
        }
    }

    public void Knockback(float knockbackForce, Vector3 senderPosition) {
            // Calculate knockback direction
            Vector3 knockbackDirection = transform.position - senderPosition;
            knockbackDirection.Normalize();

            // Apply knockback force to the Rigidbody
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(StopKnockbackAfterDuration(0.5f));
    }

    private IEnumerator StopKnockbackAfterDuration(float duration) {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Stop the knockback force
        rb.velocity = Vector2.zero;
    }

    public void Heal (int amount)
    {
        currentHealth += amount;

        StartCoroutine(DamageIndication("+"+amount, Color.blue));
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public IEnumerator DamageIndication(string damageTextAmount, Color colorType)
    {
        characterSpriteRenderer.color = colorType;
        damageText.text = damageTextAmount;
        damageText.color = colorType;
        damageTextObject.SetActive(true);
        StartCoroutine(Wait(0.2f));

        while (!indicationTimeDone)
        {
            yield return null;
        }
        
        damageTextObject.SetActive(false);
        characterSpriteRenderer.color = spriteOriginalColor;
        indicationTimeDone = false;
    }

    private IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        indicationTimeDone = true;
    }

}
