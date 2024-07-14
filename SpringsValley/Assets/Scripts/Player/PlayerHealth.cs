using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthSystem
{
    public UIManager uiManager;
    private PlayerControls playerControls;

    public float currentEnergy, maxEnergy;
    public float energyIncreaseAmount = 1f;

    public AudioSource source;
    public AudioClip lowEnergySound;

    void Start(){
        playerControls = GetComponent<PlayerControls>();
        StartCoroutine(IncreaseEnergyOverTime());
    }

    public override void Damage(int amount, GameObject sender)
    {
        // So the sender can't find itself.
        if (sender.layer == gameObject.layer)
            return;

        knockbackSender = sender;
        damageIndication(amount);

        // Checks if the object is dead
        if (currentHealth - amount < 1) {
            currentHealth = 0;
            isAlive = false;      
            
            uiManager.isAlive = false;
            playerControls.playerDeath();
            
            StartCoroutine(utility.DelayedAction(1.5f, () => 
                uiManager.ShowGameOverPanel()));
        } else if (currentHealth > 0)
        {
            currentHealth -= amount;
            isAlive = true;
        }
    }

    IEnumerator IncreaseEnergyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            IncreaseEnergy(energyIncreaseAmount);
        }
    }

    void IncreaseEnergy(float amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
    }

    public bool CanDecreaseEnergy(int amount)
    {
        if(currentEnergy >= amount) {
            currentEnergy -= amount;
            return true;
        } else {
            source.PlayOneShot(lowEnergySound);
            return false;
        }
    }
}

