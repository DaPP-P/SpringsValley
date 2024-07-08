using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

    public PlayerHealth playerHealth;
    public TextMeshProUGUI  healthText;
    public Image healthBar;
    float lerpSpeed;

    private float currentHealth;
    private float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        currentHealth = (float)playerHealth.currentHealth;
        maxHealth = (float)playerHealth.maxHealth;

        healthText.text = "Health: " + playerHealth.currentHealth;
        HealthBarFiller();
        ColourChange();
    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, lerpSpeed); 
    }

    void ColourChange()
    {
        Color healthColour = Color.Lerp(Color.red, Color.green, (currentHealth / maxHealth));
        healthBar.color = healthColour;
    }
}
