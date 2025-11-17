using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 movement = Vector2.zero;
    private Vector2 lastDirection = Vector2.down;

    [Header("Dash")]
    public float dashSpeed = 14f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.8f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;
    private Vector2 dashDir = Vector2.zero;

    [Header("Combat")]
    public Transform attackPoint;
    public float attackRadius = 0.75f;
    public float attackDamage = 5f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayers;
    private float attackTimer = 0f;

    // Expose read-only
    public bool IsDashing => isDashing;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Input
        float hx = Input.GetAxisRaw("Horizontal");
        float vy = Input.GetAxisRaw("Vertical");
        movement = new Vector2(hx, vy);

        if (movement.sqrMagnitude > 0.0001f)
            lastDirection = movement.normalized;

        // Update animator
        if (!isDashing && attackTimer <= 0f)
        {
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical", lastDirection.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }

        // Dash input
        if (Input.GetButtonDown("Dash"))
            StartDash(lastDirection);

        // Attack input
        if (Input.GetButtonDown("Slash"))
            TryAttack();

        // Timers
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;

        // Dash handling
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            rb.linearVelocity = dashDir * dashSpeed;

            if (dashTimer <= 0f)
                StopDash();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
            return;

        // Normal movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void TryAttack()
    {
        if (attackTimer > 0f || isDashing)
            return;

        attackTimer = attackCooldown;

        animator.SetBool("IsAttacking", true);
        animator.SetFloat("AttackX", lastDirection.x);
        animator.SetFloat("AttackY", lastDirection.y);

        Vector3 attackPos = attackPoint != null ? attackPoint.position : transform.position + GetAttackOffset(lastDirection);

        Debug.DrawLine(transform.position, attackPos, Color.red, 0.5f);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, attackRadius, enemyLayers);
        Debug.Log($"Attacking at {attackPos}, found {hitEnemies.Length} enemies");

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit enemy: " + enemy.name);
            EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.health -= attackDamage;
                Debug.Log($"Enemy {enemy.name} HP left: {hp.health}");
            }
        }
    }

    private Vector3 GetAttackOffset(Vector2 dir)
    {
        if (dir.y > 0.5f) return Vector3.up;
        if (dir.y < -0.5f) return Vector3.down;
        if (dir.x > 0.5f) return Vector3.right;
        if (dir.x < -0.5f) return Vector3.left;
        return dir.normalized;
    }

    public void FinishAttack() => animator.SetBool("IsAttacking", false);

    public void StartDash(Vector2 direction)
    {
        if (isDashing || cooldownTimer > 0f)
            return;

        if (direction == Vector2.zero)
            direction = lastDirection != Vector2.zero ? lastDirection : Vector2.down;

        dashDir = direction.normalized;
        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;

        animator.SetFloat("Horizontal", dashDir.x);
        animator.SetFloat("Vertical", dashDir.y);
        animator.SetBool("isDashing", true);
    }

    private void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isDashing", false);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + GetAttackOffset(lastDirection), attackRadius);
    }
}