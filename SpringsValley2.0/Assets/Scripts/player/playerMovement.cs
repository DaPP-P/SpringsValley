using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;
    public new Rigidbody2D rigidbody2D;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Attack")]
    public bool isAttacking;
    public bool attackable = true;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackHitDelay = 0.12f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1.1f;
    [SerializeField] private float attackRadius = 0.65f;
    [SerializeField] private LayerMask attackLayerMask = ~0;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip dashSound;
    public AudioClip deathSound;

    [Header("Dash")]
    public TrailRenderer trailRenderer;
    [SerializeField] private LayerMask dashLayerMask;
    public float dashAmount = 1f;

    private const string PlayerIdle = "warrior idle";
    private const string PlayerRun = "warrior run";
    private const string PlayerAttack = "warrior single swing1";
    private const string PlayerDeathState = "warrior death";

    private readonly RaycastHit2D[] dashHits = new RaycastHit2D[4];
    private readonly HashSet<HealthSystem> attackTargets = new HashSet<HealthSystem>();

    private Vector2 moveDirection;
    private Vector2 attackDirection = Vector2.right;
    private string currentState;
    private PlayerHealth playerHealth;
    private Collider2D playerCollider;
    private bool isDashButtonDown;
    private bool controlsEnabled = true;
    private float baseMoveSpeed;
    private Coroutine dashVisualRoutine;
    private Coroutine speedBoostRoutine;

    private void Awake()
    {
        if (rigidbody2D == null)
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }

        if (source == null)
        {
            source = GetComponentInParent<AudioSource>();
        }

        playerHealth = GetComponent<PlayerHealth>();
        playerCollider = GetComponent<Collider2D>();
        baseMoveSpeed = moveSpeed;
        attackable = true;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (!controlsEnabled || isAttacking)
        {
            moveDirection = Vector2.zero;
            return;
        }

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY += 1f;
        if (Input.GetKey(KeyCode.S)) moveY -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveX != 0f && spriteRenderer != null)
        {
            spriteRenderer.flipX = moveX < 0f;
        }

        ChangeAnimationState(moveDirection != Vector2.zero ? PlayerRun : PlayerIdle);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDashButtonDown = true;
        }

        if (Input.GetMouseButtonDown(0) && attackable)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (rigidbody2D == null)
        {
            return;
        }

        rigidbody2D.linearVelocity = controlsEnabled && !isAttacking
            ? moveDirection * moveSpeed
            : Vector2.zero;

        if (!isDashButtonDown)
        {
            return;
        }

        isDashButtonDown = false;
        if (!controlsEnabled || moveDirection == Vector2.zero || playerHealth == null || !playerHealth.CanDecreaseEnergy(10))
        {
            return;
        }

        float allowedDistance = GetAllowedDashDistance(moveDirection);
        rigidbody2D.MovePosition(rigidbody2D.position + moveDirection * allowedDistance);

        if (source != null && dashSound != null)
        {
            source.PlayOneShot(dashSound);
        }

        if (dashVisualRoutine != null)
        {
            StopCoroutine(dashVisualRoutine);
        }

        dashVisualRoutine = StartCoroutine(DashAnimationCoroutine());
    }

    public void ApplySpeedBoost(float amount, float durationSeconds)
    {
        if (amount <= 0f || durationSeconds <= 0f)
        {
            return;
        }

        if (speedBoostRoutine != null)
        {
            StopCoroutine(speedBoostRoutine);
        }

        speedBoostRoutine = StartCoroutine(SpeedBoostCoroutine(amount, durationSeconds));
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
        attackable = enabled;

        if (!enabled)
        {
            moveDirection = Vector2.zero;
            isDashButtonDown = false;
            if (rigidbody2D != null)
            {
                rigidbody2D.linearVelocity = Vector2.zero;
            }
        }
    }

    public void PlayerDeath()
    {
        SetControlsEnabled(false);
        CancelInvoke(nameof(PerformAttack));
        CancelInvoke(nameof(AttackComplete));
        isAttacking = false;

        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.Play(PlayerDeathState);
            currentState = PlayerDeathState;
        }

        if (source != null && deathSound != null)
        {
            source.PlayOneShot(deathSound);
        }
    }

    // Compatibility for existing Inspector events.
    public void playerDeath()
    {
        PlayerDeath();
    }

    public void OnAttackAnimationComplete()
    {
        CancelInvoke(nameof(AttackComplete));
        AttackComplete();
    }

    private void Attack()
    {
        if (isAttacking || !controlsEnabled)
        {
            return;
        }

        isAttacking = true;
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            attackDirection = ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;
        }

        if (attackDirection == Vector2.zero)
        {
            attackDirection = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
        }

        if (spriteRenderer != null && Mathf.Abs(attackDirection.x) > 0.01f)
        {
            spriteRenderer.flipX = attackDirection.x < 0f;
        }

        ChangeAnimationState(PlayerAttack);
        Invoke(nameof(PerformAttack), Mathf.Clamp(attackHitDelay, 0f, attackDelay));
        Invoke(nameof(AttackComplete), Mathf.Max(attackDelay, 0.01f));
    }

    private void PerformAttack()
    {
        Vector2 hitPosition = (Vector2)transform.position + attackDirection * attackRange;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPosition, attackRadius, attackLayerMask);
        attackTargets.Clear();

        foreach (Collider2D hit in hits)
        {
            HealthSystem target = hit.GetComponentInParent<HealthSystem>();
            if (target != null && target.transform.root != transform.root && attackTargets.Add(target))
            {
                target.Damage(attackDamage, gameObject);
            }
        }
    }

    private void AttackComplete()
    {
        isAttacking = false;
    }

    private float GetAllowedDashDistance(Vector2 direction)
    {
        if (playerCollider == null || dashLayerMask.value == 0)
        {
            return dashAmount;
        }

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(dashLayerMask);
        contactFilter.useTriggers = false;

        int hitCount = playerCollider.Cast(direction, contactFilter, dashHits, dashAmount);
        if (hitCount == 0)
        {
            return dashAmount;
        }

        float closestDistance = dashAmount;
        for (int index = 0; index < hitCount; index++)
        {
            closestDistance = Mathf.Min(closestDistance, dashHits[index].distance);
        }

        return Mathf.Max(0f, closestDistance - 0.02f);
    }

    private void ChangeAnimationState(string newState)
    {
        if (animator == null || currentState == newState)
        {
            return;
        }

        animator.Play(newState);
        currentState = newState;
    }

    private IEnumerator DashAnimationCoroutine()
    {
        if (trailRenderer == null)
        {
            yield break;
        }

        trailRenderer.enabled = true;
        yield return new WaitForSeconds(0.3f);
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        dashVisualRoutine = null;
    }

    private IEnumerator SpeedBoostCoroutine(float amount, float durationSeconds)
    {
        moveSpeed = baseMoveSpeed + amount;
        yield return new WaitForSeconds(durationSeconds);
        moveSpeed = baseMoveSpeed;
        speedBoostRoutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 previewDirection = attackDirection == Vector2.zero ? Vector2.right : attackDirection;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + previewDirection * attackRange, attackRadius);
    }
}
