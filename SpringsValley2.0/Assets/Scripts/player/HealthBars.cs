using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBars : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public TextMeshProUGUI healthText;
    public Image healthBar;
    public Image energyBar;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (playerHealth == null)
        {
            return;
        }

        float healthPercentage = playerHealth.GetHealthPercentage();
        float energyPercentage = playerHealth.maxEnergy > 0f
            ? Mathf.Clamp01(playerHealth.currentEnergy / playerHealth.maxEnergy)
            : 0f;
        float lerpAmount = Mathf.Clamp01(3f * Time.unscaledDeltaTime);

        if (healthText != null)
        {
            healthText.text = playerHealth.currentHealth.ToString();
        }

        if (healthBar != null)
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, healthPercentage, lerpAmount);
            healthBar.color = Color.Lerp(Color.red, Color.green, healthPercentage);
        }

        if (energyBar != null)
        {
            energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount, energyPercentage, lerpAmount);
            energyBar.color = Color.Lerp(Color.white, Color.blue, energyPercentage);
        }
    }
}
