using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

    public PlayerHealth playerHealth;
    public TextMeshProUGUI healthText;

    public TextMeshProUGUI cointText; 
    public Image healthBar;

    public Image energyBar;
    float lerpSpeed;

    private float currentHealth;
    private float maxHealth;

    private float currentEnergy;
    private float maxEnergy;

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
        currentEnergy = (float)playerHealth.currentEnergy;
        maxEnergy = (float)playerHealth.maxEnergy;
        

        healthText.text = "Health: " + playerHealth.currentHealth;
        cointText.text = game.coinCount.ToString() + " x ";
        
        HealthBarFiller();
        EnergyBarFiller();
        ColourChange();
    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, lerpSpeed); 
    }

    void EnergyBarFiller()
    {
        energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount, currentEnergy / maxEnergy, lerpSpeed); 

    }

    void ColourChange()
    {
        Color healthColour = Color.Lerp(Color.red, Color.green, (currentHealth / maxHealth));
        healthBar.color = healthColour;

        Color energyColour = Color.Lerp(Color.white, Color.blue, (currentEnergy / maxEnergy));
        energyBar.color = energyColour;
    }

}
