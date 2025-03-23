using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerHealth : HealthSystem
{

    /* Object Refernces */
    public UIManager uiManger;
    private PlayerMovement playerMovement;


    /* Energy */
    public float currentEnergy, maxEnergy;
    public float energyIncreaseAmount = 1f;

    public static int health;

    /* Audio */
    public AudioSource source;
    public AudioClip lowEnergySound;
    public AudioClip damageSound;

    void Start(){
        playerMovement = GetComponent<PlayerMovement>();
        StartCoroutine(IncreaseEnergyOverTime());

        health = 50;
    }


    /* Energy methods */
    IEnumerator IncreaseEnergyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            IncreaseEnergy(energyIncreaseAmount);
        }
    }

    public void IncreaseEnergy(float amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
    }
    
    public void DecreaseEnergy(float amount)
    {
        currentEnergy = Mathf.Min(currentEnergy - amount);
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

    public bool BasicCanDecreaseEngery(int amount)
    {
      if(currentEnergy >= amount) {
            return true;
        } else {
            source.PlayOneShot(lowEnergySound);
            return false;
        }
    }


}
