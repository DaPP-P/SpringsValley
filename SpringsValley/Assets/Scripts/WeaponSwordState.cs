using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwordState : WeaponBaseState
{
    private WeaponStateManager weapon;
    private SwordStats swordStats;
    private Animator swordAnimator;
    private int attackCount;
    private int damageAmount;
    private bool attackBlocked;


    public bool isAttacking { get; private set; }
    public int offset = 0;

    private List<GameObject> hitObjects = new List<GameObject>();

    public override void EnterState(WeaponStateManager weapon)
    {
        this.weapon = weapon;
        Debug.Log("hello from sword state");
        weapon.InstantiateSwordPrefab();
        attackCount = 0;

        swordStats = weapon.currentWeaponInstance.GetComponent<SwordStats>();
        swordAnimator = weapon.currentWeaponInstance.GetComponent<Animator>();    
        weapon.weaponRenderer = weapon.currentWeaponInstance.GetComponent<SpriteRenderer>();
    
    }

    public override void UpdateState(WeaponStateManager weapon)
    {
        followMouse();

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon.DestroyCurrentInstance(); // I want to destroy the instantiate prefab
            weapon.SwitchState(weapon.bowState);
        }
    }

    public void swordAttack()
    {
        if (attackBlocked)
            return;

        damageAmount = swordStats.leftClickDamageAmount;

        if (attackCount%2 == 0) {
            swordAnimator.SetTrigger("AttackDown");
        } 
        else 
        {
            swordAnimator.SetTrigger("AttackUp");
        }
        
        attackBlocked = true;
        isAttacking = true;
        attackCount += 1;
        weapon.StartCoroutine(DelayAttack(0.3f));
    }

    public void swordSpecialAttack()
    {
        Debug.Log("Special Attack");

        if (attackBlocked)
            return;
        
        /* Need to convert damageAmount to float and then round so damage
         * amount can be leftClickDamage * 1.5 */
        damageAmount = swordStats.leftClickDamageAmount;

        swordAnimator.SetTrigger("StabAttack");

        attackBlocked = true;
        isAttacking = true;
        weapon.StartCoroutine(DelayAttack(0.4f));
    }

    private IEnumerator DelayAttack(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        attackBlocked = false;
    }

    // I have no idea why I cant follow 'currentWeaponInstance' but whatever it worksish
    public void followMouse()
    {
        // Converts the mouse coordinates to in game coordinates and finds the difference between
        // there and the weapon.hand.
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.hand.transform.position;
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

        if ( weapon.hand.transform.eulerAngles.z > 0 &&  weapon.hand.transform.eulerAngles.z < 180)
        {
            weapon.weaponRenderer.sortingOrder = weapon.characterRenderer.sortingOrder - 1;
        } 
        else 
        {
            weapon.weaponRenderer.sortingOrder = weapon.characterRenderer.sortingOrder + 1;
        }
    }

    public void ResetAttack()
    {
        hitObjects.Clear();
        isAttacking = false;
        Debug.Log("reset attack");
    }

    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(weapon.circleOrigin.position, weapon.radius))
        {
            HealthSystem healthSystem;
            if ((healthSystem = collider.GetComponent<HealthSystem>()) != null) // && !hitObjects.Contains(collider.gameObject)
            {
                Debug.Log("I hit " + collider.gameObject.name);
                Debug.Log(weapon.currentWeaponInstance);
                healthSystem.Damage(damageAmount, weapon.currentWeaponInstance);
                hitObjects.Add(collider.gameObject);
   
            }
        }
    }

}
