using UnityEngine;

public abstract class WeaponBaseState
{
    public abstract void EnterState(WeaponStateManager weapon);

    public abstract void UpdateState(WeaponStateManager weapon);
}
