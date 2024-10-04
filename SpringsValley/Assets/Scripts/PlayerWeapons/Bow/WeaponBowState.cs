using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBowState : WeaponBaseState
{
    private WeaponStateManager weapon; // Instance to the Weapon State Manager.
    private BowStats bowStats; // The Bow stats
    private Animator bowAnimator; // The animation for the bow.

    private int damageAmount; // The amount of damage an attack should do.
    private float delay = 0.4f; // The delay between attacks. //TODO: THINK I CAN DELETE.
    private bool attackBlocked; // Boolean of whether or not the attack is blocked.
    public int offset = 0; //TODO: Should not need.
    private bool isAttacking = false; // Boolean of whether not the player is attacking.

    float timer;
    float holdDur = 0.8f;

    float specialTimer;
    float specialHoldDur = 1.2f;

    public PlayerHealth playerHealth;

    private List<GameObject> hitObjects = new List<GameObject>();

    /* 
     * Setup needed when WeaponBowState is loaded.
     */
    public override void EnterState(WeaponStateManager weapon)
    {
        Debug.Log("hello from bow state");
        this.weapon = weapon;
        weapon.InstantiateBowPrefab();

        bowStats = weapon.currentWeaponInstance.GetComponent<BowStats>();
        bowAnimator = weapon.currentWeaponInstance.GetComponent<Animator>(); 
        playerHealth = GameObject.Find("Main_Character").GetComponent<PlayerHealth>();

    }

    public override void UpdateState(WeaponStateManager weapon)
    {
        if (!UIManager.isPaused) {
            followMouse();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon.DestroyCurrentInstance();
            weapon.SwitchState(weapon.swordState);
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            bowAnimator.SetTrigger("bowInterupt");
        }
    }

    // Add a delay to bow attack
    public void bowAttack()
    {

        float animationChange = 0.4f;

        if(Input.GetMouseButtonDown(0))
        {
            timer = Time.time;
            weapon.playDrawbackSound();
            
        }
        else if (Input.GetMouseButton(0))
        {
            bowAnimator.SetTrigger("bowPull");
            if (Time.time - timer > animationChange)
            {
                bowAnimator.SetTrigger("bowShoot");
            }

            if (Time.time - timer > holdDur)
            {
                timer = Time.time;
                bowAnimator.SetTrigger("bowInterupt");
                weapon.InstantiateArrowPrefab();
            }
        }
        else
        {
            timer = float.PositiveInfinity;
            bowAnimator.SetTrigger("bowInterupt");
        }
    }

    public void bowSpecialAttack()
    {

        //TODO: MAKE IT ONLY CHECK ONCE.
        if (playerHealth.BasicCanDecreaseEngery(15) == false) {
            return;
        } else {

        float animationChange = 0.7f;

        if(Input.GetMouseButtonDown(1))
        {
            specialTimer = Time.time;
            weapon.playDrawbackSound();

        }
        else if (Input.GetMouseButton(1))
        {
            bowAnimator.SetTrigger("bowPull");
            if (Time.time - timer > animationChange)
            {
                bowAnimator.SetTrigger("bowShoot");
                //playerHealth.DecreaseEnergy(15);
            }

            if (Time.time - specialTimer > specialHoldDur)
            {
                specialTimer = Time.time;
                weapon.InstantiateArrowPrefab();
                weapon.InstantiateArrowPrefab(10);
                weapon.InstantiateArrowPrefab(-10);
                bowAnimator.SetTrigger("bowInterupt");
                playerHealth.DecreaseEnergy(15);
            }
        }
        else
        {
            specialTimer = float.PositiveInfinity;
            bowAnimator.SetTrigger("bowInterupt");
        }
        }
    }

    // Simple Method for following the mouse.
    public void followMouse()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.hand.transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
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

    // Simple method to check detection.
    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(weapon.circleOrigin.position, weapon.radius))
        {
            HealthSystem healthSystem;
           
           if ((healthSystem = collider.GetComponent<HealthSystem>()) != null)
            {
                healthSystem.Damage(damageAmount, weapon.currentWeaponInstance);
            }
        }
    }
}
