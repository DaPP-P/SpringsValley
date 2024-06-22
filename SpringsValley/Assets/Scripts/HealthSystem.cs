using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    // health stats
    [SerializeField]
    private int currentHealth, maxHealth;
    
    // TODO: Use isDead to trigger death animation
    [SerializeField]
    public bool isDead = false;
    
    // TODO: This is to do with dealing with taking damage. Make it so it creates a new object and looks better.
    public GameObject damageTextObject;
    private TextMeshPro damageText;
    private SpriteRenderer characterSpriteRenderer;
    private Color spriteOriginalColor;
    public GameObject mainSprite;

    // TODO: Make KnockbackForce something that comes from the sender objects weapon.
    public float knockbackForce;
    
    public Rigidbody2D rb;

    public bool indicationTimeDone = false;

    // Need Sketelon State Manager so skeleton can be set to an idle state whie in knockback state.
    // TODO: Only call and use skeletonStateManger if the object is of type skeleton.
    private SkeletonStateManager skeletonStateManager;

    public HealthBar healthBar;

    public AudioSource source;
    public AudioClip hitSound;
    
    ///// public event EventHandler<DamageEventArgs> OnTakeDamage;
    //// public event EventHandler<HealEventArgs> OnHeal;

    public Animator animator; // The animator to play the players trail.


    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    /** Start Method
     ** Initialize Health System, items to deal with taking damage, rigidbody componet, and skeleton state manager.
     * TODO: Make it so taking damage doesn't need to be initialised like this and creates a new object each time. 
     **/
    void Start()
    {
        // Initialize the health system and find components needed for damage indications.
        InitializeHealthSystem(maxHealth);
        damageText = damageTextObject.GetComponent<TextMeshPro>();
        characterSpriteRenderer = mainSprite.GetComponent<SpriteRenderer>();
        spriteOriginalColor = characterSpriteRenderer.color;
        
        // Rigidbody to set velocity.
        rb = GetComponent<Rigidbody2D>();

        // SkeletonStateManger is needed for setting Skeleton to not move forward when being knockbacked.
        skeletonStateManager = GetComponent<SkeletonStateManager>();
    }

    
    /**
     ** Initialize Healthsystem Method
     * @param `initalise health of the object` 
     */
    public void InitializeHealthSystem(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;
    }

    /**
     ** Take Damage Method
     * TODO: Currently when the player is killed the healthbar does not get deleted. Fix it.
     * @param `Damage Amount` and `which object hit the object`
     */
    public void Damage (int amount, GameObject sender) 
    {
        // So the sender can't find itself.
        if (sender.layer == gameObject.layer)
            return;

        // sets currentHealth to currentHealth - damageTaken
        currentHealth -= amount;
        source.PlayOneShot(hitSound);

        // TODO: Remove how this is done. Stated Above.
        StartCoroutine(DamageIndication("-"+amount, Color.red));   

        if (currentHealth < 1) {
            animator.SetTrigger("Die");
            // NEED TO SET TIMER THEN DESTORY OBJECT


            //isDead = true;
            //healthBar.healthBar.fillAmount = 0;
            //Destroy(gameObject);
        }

        // Gives knockback to the hit object. Else kills and destroys the object.
        if (currentHealth > 0)
        {
             Debug.Log("Sender position: " + sender.transform.position);
             Knockback(knockbackForce, sender.transform.position);
        } else {
            //Destroy(gameObject); DONT KNOW ABOUT 
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

            // Make it so the hit enemy does not try to move
            if (gameObject.CompareTag("SkeletonEnemy")) {
                skeletonStateManager.SwitchState(skeletonStateManager.nothingState);
            }
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
        if (gameObject.CompareTag("SkeletonEnemy")) {
            skeletonStateManager.SwitchState(skeletonStateManager.pursuingState);
        }
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

        // TODO Get rid of.
        StartCoroutine(DamageIndication("+"+amount, Color.blue));
        
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

    /**
    * TODO: This needs to be redone for the final time
    */
    public IEnumerator DamageIndication(string damageTextAmount, Color colorType)
    {
        characterSpriteRenderer.color = colorType;
        damageText.text = damageTextAmount;
        damageText.color = colorType;
        damageTextObject.SetActive(true);
        StartCoroutine(Wait(0.2f));

        while (!indicationTimeDone)
        {
            yield return null;
        }
        
        damageTextObject.SetActive(false);
        characterSpriteRenderer.color = spriteOriginalColor;
        indicationTimeDone = false;
    }

    /**
     ** Helper method to easily wait for a duration
     */
    private IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        indicationTimeDone = true;
    }

}
