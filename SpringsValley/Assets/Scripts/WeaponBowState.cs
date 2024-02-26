using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBowState : WeaponBaseState
{
    private WeaponStateManager weapon;
    private BowStats bowStats;
    private Animator bowAnimator;

    private int damageAmount;
    private float delay = 0.4f;
    private bool attackBlocked;
    public int offset = 0;
    private bool isAttacking = false;

    float timer;
    float holdDur = 0.8f;

    float specialTimer;
    float specialHoldDur = 1.2f;

    private List<GameObject> hitObjects = new List<GameObject>();

    public override void EnterState(WeaponStateManager weapon)
    {
        Debug.Log("hello from bow state");
        this.weapon = weapon;
        weapon.InstantiateBowPrefab();

        bowStats = weapon.currentWeaponInstance.GetComponent<BowStats>();
        bowAnimator = weapon.currentWeaponInstance.GetComponent<Animator>(); 
    }

    public override void UpdateState(WeaponStateManager weapon)
    {
        followMouse();

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
                weapon.InstantiateArrowPrefab();
                bowAnimator.SetTrigger("bowInterupt");
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
        float animationChange = 0.7f;

        if(Input.GetMouseButtonDown(1))
        {
            specialTimer = Time.time;
        }
        else if (Input.GetMouseButton(1))
        {
            bowAnimator.SetTrigger("bowPull");
            if (Time.time - timer > animationChange)
            {
                bowAnimator.SetTrigger("bowShoot");
            }

            if (Time.time - specialTimer > specialHoldDur)
            {
                specialTimer = Time.time;
                weapon.InstantiateArrowPrefab();
                weapon.InstantiateArrowPrefab(10);
                weapon.InstantiateArrowPrefab(-10);
                bowAnimator.SetTrigger("bowInterupt");
            }
        }
        else
        {
            specialTimer = float.PositiveInfinity;
            bowAnimator.SetTrigger("bowInterupt");

        }
    }

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

    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(weapon.circleOrigin.position, weapon.radius))
        {
            HealthSystem healthSystem;
            if ((healthSystem = collider.GetComponent<HealthSystem>()) != null) // && !hitObjects.Contains(collider.gameObject)
            {
                Debug.Log("I hit " + collider.gameObject.name);
                healthSystem.Damage(damageAmount, weapon.currentWeaponInstance);
                hitObjects.Add(collider.gameObject);
            }
        }
    }
}
