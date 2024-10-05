using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHealth : HealthSystem
{

    // Enemey Audio
    public AudioSource source;
    public AudioClip deathSound;
    public AudioClip damageSound;

    public void onDeath() {

    }

    public void BasicDamage(int amount)
    {
        // Checks if the object is dead
        if (currentHealth < 1) {
            source.PlayOneShot(deathSound);
            isAlive = false;
            Destroy(gameObject);
        }
        currentHealth -= amount;
        Debug.Log("basicDamage");
        damageIndication(amount, false);
    }

    public override void Damage(int amount, GameObject sender)
    {
        // So the sender can't find itself.
        if (sender.layer == gameObject.layer)
            return;

        knockbackSender = sender;

        Debug.Log("taking damage");

        if (isAlive) {
        Debug.Log("is alive");
        damageIndication(amount);
        }

        // Checks if the object is dead
        if ((currentHealth - amount < 1) && isAlive) {
            currentHealth = 0;
            isAlive = false;    
           source.PlayOneShot(deathSound);

        } else if (currentHealth > 0)
        {
            currentHealth -= amount;
            isAlive = true;
        }
    }

    protected override void damageIndication(int damageAmount, bool isKnockback = true)
    {
        Debug.Log("WTF");
        source.PlayOneShot(damageSound);
        spriteRenderer.color = Color.red;
        DamagePopup.Create(transform.position, damageAmount);
        if (knockbackSender != null && knockbackSender != testobject && isKnockback) {
            Knockback(knockbackForce, knockbackSender.transform.position);
        }

        StartCoroutine(utility.DelayedAction(0.2f, () =>
        {
            spriteRenderer.color = Color.white;
        }));
    }

    
}
