using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameHandler : MonoBehaviour
{
    public Image healthBar;

    private HealthSystem healthSystem;

    // Start is called before the first frame update
    void Start()
    {
        healthSystem = new HealthSystem(100);
        healthSystem.OnHealthChanged += HandleHealthChanged;
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

    void HandleHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        float normalizedHealth = healthSystem.GetHealthPercentage();
        healthBar.fillAmount = normalizedHealth;
    }
}


