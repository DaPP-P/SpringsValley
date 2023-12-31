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
            OnHitWithReference?.Invoke(sender);
        }
        else
        {
            OnDeathWithReference.Invoke(sender);
            isDead = true;
            Destroy(gameObject);
        }
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
