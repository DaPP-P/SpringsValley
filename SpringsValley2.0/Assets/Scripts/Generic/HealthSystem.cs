using System;
using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    // Retained for scene/prefab compatibility. Prefer sender/root checks for real damage filtering.
    public GameObject testobject;

    public bool isAlive = true;
    public Sprite spriteOnFire;
    public Sprite orignalSprite;

    [SerializeField] public int currentHealth;
    [SerializeField] public int maxHealth;

    public float knockbackForce;
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject knockbackSender;
    public SpriteRenderer spriteRenderer;
    public GameObject player;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private Color originalColour = Color.white;
    private Coroutine fireRoutine;

    protected virtual void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (spriteRenderer != null)
        {
            originalColour = spriteRenderer.color;
            if (orignalSprite == null)
            {
                orignalSprite = spriteRenderer.sprite;
            }
        }

        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        isAlive = currentHealth > 0;
    }

    public void InitializeHealthSystem(int healthValue)
    {
        maxHealth = Mathf.Max(1, healthValue);
        currentHealth = maxHealth;
        isAlive = true;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void BasicDamage(int amount)
    {
        ApplyDamage(amount, null, false);
    }

    public virtual void Damage(int amount, GameObject sender)
    {
        ApplyDamage(amount, sender, true);
    }

    protected bool ApplyDamage(int amount, GameObject sender, bool applyKnockback)
    {
        if (!isAlive || amount <= 0 || IsSelfOrSameActor(sender))
        {
            return false;
        }

        knockbackSender = sender;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        damageIndication(amount, applyKnockback);
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
        {
            isAlive = false;
            Die(sender);
        }

        return true;
    }

    protected virtual void damageIndication(int damageAmount, bool isKnockback = true)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        DamagePopup.Create(transform.position, damageAmount);

        if (isKnockback && knockbackSender != null && knockbackSender != testobject)
        {
            Knockback(knockbackForce, knockbackSender.transform.position);
        }

        StartCoroutine(RestoreDamageColour());
    }

    protected virtual void Die(GameObject sender)
    {
        NotifyDied();
        Destroy(gameObject, 0.2f);
    }

    protected void NotifyDied()
    {
        Died?.Invoke();
    }

    public void Knockback(float force, Vector3 senderPosition)
    {
        if (rb == null || force <= 0f)
        {
            return;
        }

        Vector2 knockbackDirection = ((Vector2)transform.position - (Vector2)senderPosition).normalized;
        rb.AddForce(knockbackDirection * force, ForceMode2D.Impulse);
        StartCoroutine(StopKnockbackAfterDuration(0.3f));
    }

    public void Heal(int amount)
    {
        if (!isAlive || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public float GetHealthPercentage()
    {
        return maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    }

    public void onFire(int amount, GameObject sender)
    {
        if (!isAlive || amount <= 0 || IsSelfOrSameActor(sender))
        {
            return;
        }

        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
        }

        fireRoutine = StartCoroutine(FireTick(0.5f, amount, sender));
    }

    private bool IsSelfOrSameActor(GameObject sender)
    {
        return sender != null && (sender == gameObject || sender.transform.root == transform.root);
    }

    private IEnumerator RestoreDamageColour()
    {
        yield return new WaitForSeconds(0.2f);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColour;
        }
    }

    private IEnumerator StopKnockbackAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator FireTick(float interval, int amount, GameObject sender)
    {
        if (spriteRenderer != null && spriteOnFire != null)
        {
            spriteRenderer.sprite = spriteOnFire;
        }

        for (int damageCount = 0; damageCount < 5 && isAlive; damageCount++)
        {
            yield return new WaitForSeconds(interval);
            ApplyDamage(amount, sender, false);
        }

        if (spriteRenderer != null && orignalSprite != null)
        {
            spriteRenderer.sprite = orignalSprite;
        }

        fireRoutine = null;
    }
}
