using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStateManager : MonoBehaviour
{

    public WeaponBaseState currentState;
    public WeaponBowState bowState = new WeaponBowState();
    public WeaponSwordState swordState = new WeaponSwordState();

    public GameObject swordPrefab;
    public GameObject bowPrefab;
    public Transform hand;

    public GameObject currentWeaponInstance;

    // Start is called before the first frame update
    void Start()
    {
        currentState = swordState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        //currentState.UpdateState(this);
    }

    public void SwitchState(WeaponBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void InstantiateSwordPrefab()
    {
        // Instantiate the sword prefab at the hand position
        currentWeaponInstance = Instantiate(swordPrefab, hand.position, hand.rotation, hand);
    }

    public void InstantiateBowPrefab()
    {
        // Instantiate the bow prefab at the hand position
        currentWeaponInstance = Instantiate(bowPrefab, hand.position, hand.rotation, hand);
    }

    public void DestroyCurrentInstance()
    {
        Destroy(currentWeaponInstance);
    } 

    public void Attack()
    {
        // Check if the current state is the sword state
        if (currentState is WeaponSwordState)
        {
            // Cast currentState to WeaponSwordState and call the swordAttack method
            ((WeaponSwordState)currentState).swordAttack();
        }
        else if (currentState is WeaponBowState)
        {
            ((WeaponBowState)currentState).bowAttack();
        }
    }
}
