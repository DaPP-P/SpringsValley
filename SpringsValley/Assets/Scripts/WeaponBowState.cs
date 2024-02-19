using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBowState : WeaponBaseState
{
    private WeaponStateManager weapon;
    private BowStats bowStats;

    private int damageAmount;
    private float delay = 0.4f;
    private bool attackBlocked;
    public int offset = 0;

    private List<GameObject> hitObjects = new List<GameObject>();

    public override void EnterState(WeaponStateManager weapon)
    {
        Debug.Log("hello from bow state");
        this.weapon = weapon;
        weapon.InstantiateBowPrefab();
    }

    public override void UpdateState(WeaponStateManager weapon)
    {
        followMouse();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon.DestroyCurrentInstance();
            weapon.SwitchState(weapon.swordState);
        }
    }

    public void bowAttack()
    {
        Debug.Log("bow attack");
        weapon.InstantiateArrowPrefab();
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
