using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
TODO LIST:
* //// Remove the debug error stopping the game from running.
* //// Add comments to PlayerControls
* //// Add comments to weaponStateManager 
* Add comments to WeaponBowState and WeaponSwordState.
* //// Add CheckLineOfSight to SkeletonStateManager.
* Not worth it. ////Maybe add A* to SkeletonStateManger, as all three use it.
* ////Add Target position update every x secs instead of just when reaching end.
* For later. ////Use Path finding in IdleState.
* ////Use Path finding when retreating and add some logic to make it smoother.
    * ////Started on this. Need to change it so go back to default settings after 10 distance from spawn.
* //// Make Skeleton not be able to go through walls. Probably using rigidbody.
* Will do later date not that important. ////Add more universal variables to SkeletonStateManger.
* ////Add comments to SkeletonStateManager.
* Fuck it not important it works but to make things cleaner will needed to be done////Fix BowStats and SwordStats. 
* DISABLED STAB ATTACK IN PLAYER CONTROLS CAN NOT BE BOTHERED FIXING ITS A MESS////Fix stab attack.
* Super basic but just have somethingish. have heaps of bugs to work through ////GET ONTO COMBAT.
* this will be done in clean up ////Delete things I can delete.
* UI baby
    * Esc key opens menu. Resume and Quit btn
    * On Death. Restart and Quit btn
    * On Win. Restart and Quit btn.
        * Win will happen GameObject(skeletonEnemy == 0)
*/

public class SkeletonStateManager : MonoBehaviour
{
    public SkeletonBaseState currentState;
    public SkeletonIdleState idleState = new SkeletonIdleState();
    public SkeletonPursuingState pursuingState = new SkeletonPursuingState();
    public SkeletonAttackingState attackingState = new SkeletonAttackingState();
    public SkeletonNothingState nothingState = new SkeletonNothingState();

    public SpriteRenderer characterRenderer;
    public GameObject detectionGameObject;
    public CircleCollider2D detectionCollider;
    public GameObject exclamationPoint;


    // The original spawn position of the skeleton.
    public static Vector3 originalPosition = Vector3.zero;
   
    public bool retreating;


    // Start is called before the first frame update
    void Start()
    {
        detectionCollider = detectionGameObject.GetComponent<CircleCollider2D>();
        currentState = idleState;
        currentState.EnterState(this);
    }

    void OnCollisionEnter(Collision collision) 
    {
        currentState.OnCollisionEnter(this, collision);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);

    }

    public void SwitchState(SkeletonBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    /*
     * Method to check if the skeleton has a line of sight with the player
     */
    public bool CheckLineOfSight(GameObject player)
    {
        // Invert the enemy layer mask to exclude it so it doesn't get in the way of the LoS
        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        int layerMask = ~enemyLayerMask;
        
        /*
         * Sets up three ray casts. One aiming at the center of the player,
         * one at the bottom of the player and one at the top of the player.
         */
        RaycastHit2D rayCenter = Physics2D.Raycast(this.transform.position, player.transform.position - this.transform.position, 12, layerMask);
        Vector3 modifiedDirectionTop = (player.transform.position + Vector3.up * 0.5f) - this.transform.position;
        RaycastHit2D rayTop = Physics2D.Raycast(this.transform.position, modifiedDirectionTop, 12, layerMask);
        Vector3 modifiedDirectionBottom = (player.transform.position + Vector3.up * -0.5f) - this.transform.position;
        RaycastHit2D rayBottom = Physics2D.Raycast(this.transform.position, modifiedDirectionBottom, 12, layerMask);

        // Checks center ray cast if hits returns true.
        if ((rayCenter.collider != null) && (rayCenter.collider.CompareTag("Player")))
        {
            Debug.DrawRay(this.transform.position, player.transform.position - this.transform.position, Color.green);
            return true;
        } else {
            Debug.DrawRay(this.transform.position, player.transform.position - this.transform.position, Color.red);
        }

        // Checks top ray cast if hits returns true.
        if ((rayTop.collider != null) && rayTop.collider.CompareTag("Player"))
        {
            Debug.DrawRay(this.transform.position, modifiedDirectionTop, Color.green);
            return true;
            } else {
            Debug.DrawRay(this.transform.position, modifiedDirectionTop, Color.red);
        }

        // Checks bottom ray cast if hits returns true.
        if ((rayBottom.collider != null) && rayBottom.collider.CompareTag("Player"))
        {
            Debug.DrawRay(this.transform.position, modifiedDirectionBottom, Color.green);
            return true;
        } else {
            Debug.DrawRay(this.transform.position, modifiedDirectionBottom, Color.red);
        }

        // returns false if no hits.
        return false;
    }

    /*
     * Method for checking how far away the skeleton is from its spawn
     * If further then distance returns true
     */
    public bool CheckDistanceFromSpawn(float distance) {
       if (Vector3.Distance(this.transform.position, originalPosition) > distance) {
        return true;
       } else 
       {
        return false;
       }
    }
}
