using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBowState : WeaponBaseState
{
    private WeaponStateManager weapon;

    public override void EnterState(WeaponStateManager weapon)
    {
        Debug.Log("hello from bow state");
        this.weapon = weapon;
        weapon.InstantiateBowPrefab();

    }

    public override void UpdateState(WeaponStateManager weapon)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon.DestroyCurrentInstance();
            weapon.SwitchState(weapon.swordState);
        }
    }

    public void bowAttack()
    {
        Debug.Log("bow attack");
    }
}
