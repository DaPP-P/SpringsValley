using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHelper : MonoBehaviour
{
    public UnityEvent OnAnimationEventTriggered, OnAttackPreformed;

    public WeaponStateManager weaponStateManager;

    private void Start()
    {
        weaponStateManager = GetComponentInParent<Transform>().GetComponentInParent<WeaponStateManager>();
        if (weaponStateManager == null)
        {
            Debug.LogError("WeaponStateManager component not found in parent objects!");
        }
        else
        {
            OnAttackPreformed.AddListener(weaponStateManager.CheckHits);
            OnAnimationEventTriggered.AddListener(weaponStateManager.ResetAttack);
        }
    }

    public void TriggerEvent()
    {
        OnAnimationEventTriggered?.Invoke();
    }

    public void TriggerAttack()
    {
        OnAttackPreformed.Invoke();
    }
}