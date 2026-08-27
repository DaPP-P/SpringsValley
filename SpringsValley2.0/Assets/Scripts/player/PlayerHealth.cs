using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : HealthSystem
{
    public UIManager uiManger;
    private PlayerMovement playerMovement;

    [Header("Energy")]
    public float currentEnergy;
    public float maxEnergy;
    public float energyIncreaseAmount = 1f;

    public static int health;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip lowEnergySound;
    public AudioClip damageSound;

    public event Action<float, float> EnergyChanged;

    private Coroutine energyRoutine;

    protected override void Awake()
    {
        base.Awake();
        playerMovement = GetComponent<PlayerMovement>();

        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }

        if (source == null)
        {
            source = GetComponentInParent<AudioSource>();
        }

        if (uiManger == null)
        {
            uiManger = FindFirstObjectByType<UIManager>();
        }

        maxEnergy = Mathf.Max(0f, maxEnergy);
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        health = currentHealth;
    }

    private void OnEnable()
    {
        HealthChanged += HandleHealthChanged;
        energyRoutine = StartCoroutine(IncreaseEnergyOverTime());
    }

    private void OnDisable()
    {
        HealthChanged -= HandleHealthChanged;
        if (energyRoutine != null)
        {
            StopCoroutine(energyRoutine);
            energyRoutine = null;
        }
    }

    public void IncreaseEnergy(float amount)
    {
        if (amount <= 0f || maxEnergy <= 0f)
        {
            return;
        }

        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
        EnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    public void DecreaseEnergy(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentEnergy = Mathf.Clamp(currentEnergy - amount, 0f, maxEnergy);
        EnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    public bool CanDecreaseEnergy(int amount)
    {
        if (amount >= 0 && currentEnergy >= amount)
        {
            DecreaseEnergy(amount);
            return true;
        }

        PlayLowEnergySound();
        return false;
    }

    public bool BasicCanDecreaseEngery(int amount)
    {
        bool canAfford = amount >= 0 && currentEnergy >= amount;
        if (!canAfford)
        {
            PlayLowEnergySound();
        }

        return canAfford;
    }

    protected override void damageIndication(int damageAmount, bool isKnockback = true)
    {
        base.damageIndication(damageAmount, isKnockback);
        if (source != null && damageSound != null)
        {
            source.PlayOneShot(damageSound);
        }
    }

    protected override void Die(GameObject sender)
    {
        health = 0;

        if (playerMovement != null)
        {
            playerMovement.PlayerDeath();
        }

        if (uiManger != null)
        {
            uiManger.ShowGameOverPanel();
        }

        NotifyDied();
    }

    private IEnumerator IncreaseEnergyOverTime()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(0.2f);
            IncreaseEnergy(energyIncreaseAmount);
        }
    }

    private void HandleHealthChanged(int newHealth, int newMaxHealth)
    {
        health = newHealth;
    }

    private void PlayLowEnergySound()
    {
        if (source != null && lowEnergySound != null)
        {
            source.PlayOneShot(lowEnergySound);
        }
    }
}
