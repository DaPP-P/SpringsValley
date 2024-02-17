using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwordState : WeaponBaseState
{
    private WeaponStateManager weapon;

    public bool isAttacking { get; private set; }
    public int offset = 0;



    public override void EnterState(WeaponStateManager weapon)
    {
        this.weapon = weapon;
        Debug.Log("hello from sword state");
        weapon.InstantiateSwordPrefab();
    }

    public override void UpdateState(WeaponStateManager weapon)
    {
        //followMouse();

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon.DestroyCurrentInstance(); // I want to destroy the instantiate prefab
            weapon.SwitchState(weapon.bowState);
        }
    }

    public void swordAttack()
    {
        Debug.Log("Attacking with sword");
    }

    public void followMouse()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.currentWeaponInstance.transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        weapon.currentWeaponInstance.transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
        Vector2 scale = weapon.currentWeaponInstance.transform.localScale;

        if(Mathf.Abs(rotation_z) > 90)
        {
            scale.y = -1;
        }else if(Mathf.Abs(rotation_z) < 90)
        {
            scale.y = 1;
        }

        weapon.currentWeaponInstance.transform.localScale = scale;
    }

}
