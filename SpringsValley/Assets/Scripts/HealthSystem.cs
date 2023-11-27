using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthSystem
{
    private int health;
    private int healthMax;
    public event EventHandler<DamageEventArgs> OnTakeDamage;
    public event EventHandler<HealEventArgs> OnHeal;


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

        OnTakeDamage?.Invoke(this, new DamageEventArgs(damageAmount));
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > healthMax) health = healthMax;

        OnHeal?.Invoke(this, new HealEventArgs(healAmount));
    }

}

public class DamageEventArgs : EventArgs
{
    public int DamageAmount { get; private set; }

    public DamageEventArgs(int damageAmount)
    {
        DamageAmount = damageAmount;
    }
}

public class HealEventArgs : EventArgs
{
    public int HealAmount { get; private set; }

    public HealEventArgs(int healAmount)
    {
        HealAmount = healAmount;
    }
}