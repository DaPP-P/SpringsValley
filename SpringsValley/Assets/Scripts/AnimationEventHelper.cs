using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//TODO: NEED TO FIGURE OUT WHATS GOING ON HERE.
public class AnimationEventHelper : MonoBehaviour
{
    public UnityEvent OnAnimationEventTriggered, OnAttackPreformed, OnSpecialAttackPreformed;

    public WeaponStateManager weaponStateManager;

    private void Start()
    {
        weaponStateManager = GetComponentInParent<Transform>().GetComponentInParent<WeaponStateManager>();
        if (weaponStateManager == null)
        {
            ////Debug.LogError("WeaponStateManager component not found in parent objects!");
        }
        else
        {
            OnAttackPreformed.AddListener(weaponStateManager.CheckHits);
            OnSpecialAttackPreformed.AddListener(weaponStateManager.CheckSpecialHits);
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

    public void TriggerSpecialAttack()
    {
        OnSpecialAttackPreformed.Invoke();
    }
}