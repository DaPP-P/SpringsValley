using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStateManager : MonoBehaviour
{

    SkeletonBaseState currentState;
    public SkeletonIdleState idleState = new SkeletonIdleState();
    public SkeletonPursuingState pursuingState = new SkeletonPursuingState();
    public SkeletonAttackingState attackingState = new SkeletonAttackingState();

    public SpriteRenderer characterRenderer;
    public GameObject detectionGameObject;
    public CircleCollider2D detectionCollider;
    public GameObject exclamationPoint;

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
}
