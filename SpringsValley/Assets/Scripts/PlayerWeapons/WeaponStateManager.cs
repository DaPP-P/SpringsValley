using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStateManager : MonoBehaviour
{
    public WeaponBaseState currentState; // The current state.
    public WeaponBowState bowState = new WeaponBowState(); // Create instance of bowState.
    public WeaponSwordState swordState = new WeaponSwordState(); // Create instance of weaponState.

    public Rigidbody2D rigidbody2D; // Players rigidbody.

    public GameObject swordPrefab; // Sword Prefab
    public GameObject bowPrefab; // Bow Prefab
    public GameObject arrowPrefab; // Arrow Prefab

    public Sprite playerTornado;
    
    public Transform hand; // Location where weapons rotate around.

    public Transform hand2;
    public GameObject currentWeaponInstance; // The current weapon.
    public GameObject arrow; // TODO: unsure.

    public Transform circleOrigin; // Hit radius.
    public float radius;

    public AudioSource source;
    public AudioClip swordAttackSound, bowAttackSound, bowDrawbackSound, swordSpecialAttackSound;

    protected GameObject activateWeaponSprite;

    // Bool so the weapon cant be changed while attacking.
    public bool weaponLock = false;

    /*
     * Setup needed when WeaponStateManager is loaded.
     */
    void Start()
    {
        // Initial Admin.
        currentState = swordState;
        currentState.EnterState(this);
        rigidbody2D = GetComponentInParent<Rigidbody2D>();
    }

    /*
     * Update method.
     */
    void Update()
    {
        currentState.UpdateState(this);
    }

    /*
     * Method for switching Weapon states.
     */
    public void SwitchState(WeaponBaseState state)
    {
        if (!weaponLock) {
            DestroyCurrentInstance();
            currentState = state;
            state.EnterState(this);
            newState();
        }
    }

    public int CheckMovementDirection() {
        if (rigidbody2D.velocity.x > 0)
        {
            return 1;
        }
        else if (rigidbody2D.velocity.x < 0)
        {
            return 2;
        } 

        if (rigidbody2D.velocity.y > 0)
        {
        }
        else if (rigidbody2D.velocity.y < 0)
        {
        }

        return 0;
    }

    private void newState() {
        //activateWeaponSprite = currentState.

    }

    /*
     * Method for instantiating the Sword prefab.
     */
    public void InstantiateSwordPrefab()
    {
        // Instantiate the sword prefab at the hand position
        currentWeaponInstance = Instantiate(swordPrefab, hand.position, Quaternion.identity, hand);
        circleOrigin = currentWeaponInstance.transform.Find("CircleOrigin");
    }

    /*
     * Method for instantiating the Bow prefab.
     */
    public void InstantiateBowPrefab()
    {
        // Instantiate the bow prefab at the hand position
        currentWeaponInstance = Instantiate(bowPrefab, hand.position, hand.rotation, hand);
    }

    /*
     * Method for instantiating the Bow prefab.
     * Instantiated when the bow is attacking.
     */
    public void InstantiateArrowPrefab(int? rotation = null)
    {   
        playAttackSound();


        // If the arrow is meant to be spawned on an rotation (special attack), add it.
        if (rotation.HasValue)
            arrow = Instantiate(arrowPrefab, hand.position, hand.rotation * Quaternion.Euler(0, 0, rotation.Value));
        // Otherwise spawn the arrow straight.
        else
            arrow = Instantiate(arrowPrefab, hand.position, hand.rotation);
    }

    public void moveWhileAttack(Vector3 mousePos)
    {
        rigidbody2D.velocity = mousePos;
        Debug.Log("should be moving");
    }

    /*
     * Method for destroying the current instance.
     */
    public void DestroyCurrentInstance()
    {
        Destroy(currentWeaponInstance);
    } 

    /*
     * Method for processing Attacks.
     */
    public void Attack()
    {
        // Check if the current state is the Sword state
        if (currentState is WeaponSwordState)
        {
            // Cast currentState to WeaponSwordState and call the swordAttack method
            ((WeaponSwordState)currentState).swordAttack();
        }
        // Check if the current state is the Bow state
        else if (currentState is WeaponBowState)
        {
            // Cast currentState to WeaponBowState and call the bowAttack method
            ((WeaponBowState)currentState).bowAttack();
        }
    }

    /*
     * Method for processing Special Attacks.
     */
    public void SpecialAttack()
    {
        // Check if the current state is the Sword state
        if (currentState is WeaponSwordState)
        {
            // Cast currentState to WeaponSwordState and call the swordAttack method
            ((WeaponSwordState)currentState).swordSpecialAttack();
        }
        // Check if the current state is the Bow state.
        else if (currentState is WeaponBowState)
        {
            // Cast currentState to WeaponBowState and call the bowAttack method.
            ((WeaponBowState)currentState).bowSpecialAttack();
        }
    }

    public void playAttackSound()
    {
        // Check if the current state is the Sword state
        if (currentState is WeaponSwordState)
        {
            // Cast currentState to WeaponSwordState and call the swordAttack method
            source.PlayOneShot(swordAttackSound);
        }
        // Check if the current state is the Bow state
        else if (currentState is WeaponBowState)
        {
            // Cast currentState to WeaponBowState and call the bowAttack method
            source.PlayOneShot(bowAttackSound);
        }
    }

    public void playSwordSpecialAttackdown()
    {
        source.PlayOneShot(swordSpecialAttackSound);
    }

    public void playDrawbackSound()
    {
        // Check if the current state is the Bow state
        if (currentState is WeaponBowState)
        {
            // Cast currentState to WeaponBowState and call the bowAttack method
            source.PlayOneShot(bowDrawbackSound);
        }
    }

    //TODO: THE THREE METHOD BELOW HAVE ZERO REFERENCES. MIGHT BE ABLE TO DELETE BUT IT MIGHT HAVE TO DO WITH THE EVENT HANDLER
    public void CheckHits()
    {
        if (currentState is WeaponSwordState)
        {
            ((WeaponSwordState)currentState).DetectColliders();
        }
    }

    public void CheckSpecialHits()
    {
        if (currentState is WeaponSwordState)
        {
            ((WeaponSwordState)currentState).SpecialDetectColliders();
        }
    }

    public void ResetAttack()
    {
        if (currentState is WeaponSwordState)
        {
            ((WeaponSwordState)currentState).ResetAttack();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }
}
