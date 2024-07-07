using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthSystem
{
    public UIManager uiManager;
    private PlayerControls playerControls;

    void Start(){
        playerControls = GetComponent<PlayerControls>();
    }

    public override void Damage(int amount, GameObject sender)
    {
        // So the sender can't find itself.
        if (sender.layer == gameObject.layer)
            return;

        // sets currentHealth to currentHealth - damageTaken
        currentHealth -= amount;

        // Checks if the object is dead
        if (currentHealth < 1) {
            isAlive = false;      
            
            uiManager.isAlive = false;
            playerControls.playerDeath();
            
            StartCoroutine(utility.DelayedAction(1.5f, () => 
                uiManager.ShowGameOverPanel()));
        }
        
    }
}
