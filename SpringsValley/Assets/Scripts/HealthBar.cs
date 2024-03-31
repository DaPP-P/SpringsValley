using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 ** Simple class for updating and displaying the healthbar.
 * TODO: destoy the healthbar if the healthsystem is zero.
*/
public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public HealthSystem healthSystem;
    void Start()
    {
        if (healthSystem != null)
        {
            UpdateHealthBar();
        }
        else
        {
            healthBar.fillAmount = 0f;
            Debug.LogError("HealthSystem instance not found!");
        }
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        // Make sure healthSystem is not null before accessing its methods
        if (healthSystem != null)
        {
            healthBar.fillAmount = healthSystem.GetHealthPercentage();
        }
    }
}

