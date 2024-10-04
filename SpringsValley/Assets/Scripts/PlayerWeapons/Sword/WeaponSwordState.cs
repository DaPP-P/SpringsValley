using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSwordState : WeaponBaseState
{
    private WeaponStateManager weapon; // Instance to the Weapon State Manager.
    private SwordStats swordStats; // The Swords stats.
    private Animator swordAnimator; // The animation for the sword.
    private int attackCount; // The amount of attacks completed.
    private int damageAmount; // The amount of damage an attack should do.
    private bool attackBlocked; // Boolean of whether or not the attack is blocked.

    public PlayerControls playerControls; // Reference to the player controls.

    public float dashDistance = 10f;

    public Vector3 difference;

    public bool isAttacking { get; private set; } //TODO: Hope i dont need.
    public int offset = 0; //TODO: Should not need.

    private List<GameObject> hitObjects = new List<GameObject>(); // A list of objects hit.

    private Animator animator;

    private GameObject swordPrefab; // Sword Prefab

    public PlayerHealth playerHealth;



    /*
     * Setup needed when WeaponSwordState is loaded.
     */
    public override void EnterState(WeaponStateManager weapon)
    {
        // Initial Admin.
        this.weapon = weapon;
        Debug.Log("hello from sword state");
        weapon.InstantiateSwordPrefab();
        attackCount = 0;

        // Gets the needed components.
        swordStats = weapon.currentWeaponInstance.GetComponent<SwordStats>();
        swordAnimator = weapon.currentWeaponInstance.GetComponent<Animator>();    
        playerControls = GameObject.Find("Main_Character").GetComponent<PlayerControls>();
        animator = GameObject.Find("Sprite").GetComponent<Animator>();
        playerHealth = GameObject.Find("Main_Character").GetComponent<PlayerHealth>();

    }

    /*
     * Update method.
     */
    public override void UpdateState(WeaponStateManager weapon)
    {
        // Keeps track of where the mouse is.
        if (!UIManager.isPaused) {
            //followMouse();
        }

        // Checks if the player switches weapon.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {   
            // Destroys the current weapon instance and switches to the other weapon.
            weapon.DestroyCurrentInstance();
            weapon.SwitchState(weapon.bowState);
        }

        if (weapon.CheckMovementDirection() == 1)
        {
            weapon.currentWeaponInstance.transform.SetParent(weapon.hand2);
        } else if (weapon.CheckMovementDirection() == 2) {
            weapon.currentWeaponInstance.transform.SetParent(weapon.hand);

        }
    }

    /*
     * Method for controlling the normal sword attack.
     */
    public void swordAttack()
    {
        // if the attack is blocked, don't attack.
        if (attackBlocked)
            return;

        followMouse();

        // As this is a normal attack set it to the normal damage amount.
        damageAmount = swordStats.leftClickDamageAmount;
        playerControls.callDashAndAttack(2, 0.15f); 
        weapon.playAttackSound();   

        // This is so the attack switches between attacking down and attacking up.
        // Plays the normal sword attack animation.
        if (attackCount%2 == 0) {
            swordAnimator.SetTrigger("AttackDown");
        } 
        else 
        {
            swordAnimator.SetTrigger("AttackUp");
        }
        
        // stops attacking and has a delay so attacking has a cool down.
        attackBlocked = true;
        isAttacking = true;
        attackCount += 1;
        weapon.StartCoroutine(DelayAttack(0.3f));

        //defaultWeaponPosition();
    }

    /*
     * Method for controlling the special sword attack.
     */
    public void swordSpecialAttack()
    {
        if (attackBlocked)
            return;

        if (playerHealth.CanDecreaseEnergy(15) == false) {
            return;
        } else {

        followMouse();


        // As this is a special attack set it to the special damage amount.
        damageAmount = swordStats.rightClickDamageAmount;

        // Plays the special sword attack animation
        weapon.playSwordSpecialAttackdown();
        swordAnimator.SetTrigger("powerAttack");
        playerControls.callDashAndAttack(6, 0.4f); 


        // stops attacking and has a delay so attacking has a cool down.
        attackBlocked = true;
        isAttacking = true;
        weapon.StartCoroutine(DelayAttack(0.8f));
        
        }
    }

    /*
     * Method so there is a call down and attacks need to be delayed.
     */
    private IEnumerator DelayAttack(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        defaultWeaponPosition();
        attackBlocked = false;
    }

    /*
     * Method for following the mouse of the player.
     I have no idea why I cant follow 'currentWeaponInstance' but whatever it worksish
     Not really sure how this works lol its old.
     */
    public void followMouse()
    {
        // Converts the mouse coordinates to in game coordinates and finds the difference between
        // there and the weapon.hand.
        difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.hand.transform.position;
        difference.Normalize();
        
        
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        rotation_z += 45;
        weapon.hand.transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
        Vector2 scale = weapon.hand.transform.localScale;

        if(Mathf.Abs(rotation_z) > 90)
        {
            scale.y = -1;
        }else if(Mathf.Abs(rotation_z) < 90)
        {
            scale.y = 1;
        }

        weapon.hand.transform.localScale = scale;
    }

    public void defaultWeaponPosition(){
        weapon.hand.transform.localRotation = Quaternion.identity;
        weapon.hand.transform.localScale = Vector3.one;
    }

    /*
     * Method to allow the player to complete another attack.
     */
    public void ResetAttack()
    {
        hitObjects.Clear();
        isAttacking = false;
        Debug.Log("reset attack");
    }

    /*
     * Method for detecting if the sword has clashed with an enemy.
     */
    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(weapon.circleOrigin.position, weapon.radius))
        {
            HealthSystem healthSystem;
            if ((healthSystem = collider.GetComponent<HealthSystem>()) != null && !hitObjects.Contains(collider.gameObject))
            {
                healthSystem.Damage(damageAmount, weapon.currentWeaponInstance);
                hitObjects.Add(collider.gameObject);
   
            }
        }
    }

     public void SpecialDetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(weapon.circleOrigin.position, weapon.radius))
        {
            HealthSystem healthSystem;
            if ((healthSystem = collider.GetComponent<HealthSystem>()) != null && !hitObjects.Contains(collider.gameObject))
            {
                healthSystem.Damage(damageAmount, weapon.currentWeaponInstance);
                healthSystem.onFire(2, weapon.currentWeaponInstance);
                hitObjects.Add(collider.gameObject);
   
            }
        }
    }

    // Make a method alongs the line of 

}
