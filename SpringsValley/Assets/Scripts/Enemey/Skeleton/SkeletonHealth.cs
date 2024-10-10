using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHealth : HealthSystem
{

    // Enemey Audio
    public AudioSource source;
    public AudioClip deathSound;
    public AudioClip damageSound;

    public GameObject coinPrefab;
    public Transform coinSpawnPoint;

    void Start()
    {
        // Rigidbody to set velocity.
        rb = GetComponent<Rigidbody2D>();
        maxHealth = 50;
        currentHealth = 50;
    }

    public void onDeath() {

    }

    void Update()
   {

    // Checks if the skeleton is dead.
    if (currentHealth < 1 && isAlive) {
        currentHealth = 0;
        isAlive = false;

        //create a prefab called coin
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        }

        // Does death things
        source.PlayOneShot(deathSound);
        Destroy(gameObject);
    }
   }

    public void BasicDamage(int amount)
    {
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

        if (currentHealth > 0)
        {
            currentHealth -= amount;
            isAlive = true;
        }
    }

    protected override void damageIndication(int damageAmount, bool isKnockback = true)
    {
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
