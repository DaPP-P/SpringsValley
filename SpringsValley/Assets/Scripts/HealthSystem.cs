using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthSystem
{
    private int health;
    private int healthMax;
    public event EventHandler OnHealthChanged;

    public HealthSystem(int healthMax)
    {
        this.healthMax = healthMax;
        this.health = healthMax;
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetHealthPercentage()
    {
        return (float)health / healthMax;
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health < 0) health = 0;

        // Notify subscribers (if any) about the health change
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > healthMax) health = healthMax;

        // Notify subscribers (if any) about the health change
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
}