using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public HealthSystem healthSystem; // Add a reference to the HealthSystem2 instance

    void Start()
    {
        // Assuming that you have a reference to the HealthSystem2 instance somewhere in your code
        // If not, you may need to find it or create it as needed
        // Example: healthSystem = FindObjectOfType<HealthSystem2>();

        if (healthSystem != null)
        {
            UpdateHealthBar();
        }
        else
        {
            Debug.LogError("HealthSystem2 instance not found!");
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

