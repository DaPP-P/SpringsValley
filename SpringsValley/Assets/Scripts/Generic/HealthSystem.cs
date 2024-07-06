using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    // bool to check if the object is alive
    protected bool isAlive = true;

    // health stats
    [SerializeField]
    public int currentHealth, maxHealth;

    // TODO: Make KnockbackForce something that comes from the sender objects weapon.
    public float knockbackForce;
    public Rigidbody2D rb;

    public Animator animator;


    /** Start Method
     ** Initialize Health System, items to deal with taking damage, rigidbody componet, and skeleton state manager.
     * TODO: Make it so taking damage doesn't need to be initialised like this and creates a new object each time. 
     **/
    void Start()
    {
        // Rigidbody to set velocity.
        rb = GetComponent<Rigidbody2D>();
   }

    
    /**
     ** Initialize Healthsystem Method
     * @param `initalise health of the object` 
     */
    public void InitializeHealthSystem(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isAlive = true;
    }


    /**
     ** Take Damage Method
     * @param `Damage Amount` and `which object hit the object`
     */
    public virtual void Damage (int amount, GameObject sender) 
    {
        // So the sender can't find itself.
        if (sender.layer == gameObject.layer)
            return;

        // sets currentHealth to currentHealth - damageTaken
        currentHealth -= amount;

        // Checks if the object is dead
        if (currentHealth < 1) {
            isAlive = false;
        }
        // Gives knockback to the hit object. Else kills and destroys the object.
        else if (currentHealth > 0)
        {
            isAlive = true;
             Debug.Log("Sender position: " + sender.transform.position);
             Knockback(knockbackForce, sender.transform.position);
        }
    }


    /**
     ** Knockback method used to apply knockback to an object.
     * TODO: Get KnockbackForce from the senders weapon stats
     * @param `The amount of knockbackForce` and `the position of the sender`
     */
    public void Knockback(float knockbackForce, Vector3 senderPosition) {
            
            // Calculate knockback direction
            Vector3 knockbackDirection = transform.position - senderPosition;
            knockbackDirection.Normalize();

            // Apply knockback force to the Rigidbody
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            // TODO: Get Knockback duration from the sender objects weapoon
            StartCoroutine(StopKnockbackAfterDuration(0.3f));
    }


    /**
     ** Method to stop knockback after a curtain duration
     * TODO: Get Knockback duration from the sender objects weapoon
     * @param `the duration of knockback`
     */
    private IEnumerator StopKnockbackAfterDuration(float duration) {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Stop the knockback force and resumes pursuing the player
        rb.velocity = Vector2.zero;
    }


    /** 
     ** Method for healing the player
     * TODO: Once again fix the DamageInication Coroutine
     * @param `the amount to heal`
     */
    public void Heal (int amount)
    {
        // heal object
        currentHealth += amount;
        
        // if health is maxed leave it maxed.
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    /** 
     ** Method to get health percentage
     */
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

}
