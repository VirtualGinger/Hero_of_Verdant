using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    public Image attackIcon;
    public Image dashIcon;

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

    public bool IsDashing => isDashing;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    /*
    void Start()
    {
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
    }
    */
    void Update()
    {
        float hx = Input.GetAxisRaw("Horizontal");
        float vy = Input.GetAxisRaw("Vertical");
        movement = new Vector2(hx, vy);

        if (movement.sqrMagnitude > 0.0001f)
            lastDirection = movement.normalized;

        if (!isDashing && attackTimer <= 0f)
        {
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical", lastDirection.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }

        if (Input.GetButtonDown("Dash"))
            StartDash(lastDirection);

        if (Input.GetButtonDown("Slash"))
            TryAttack();

        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            rb.linearVelocity = dashDir * dashSpeed;

            if (dashTimer <= 0f)
                StopDash();
        }

        if (attackIcon != null)
        {
            if (attackTimer > 0f)
                attackIcon.fillAmount = 1 - (attackTimer / attackCooldown);
            else
                attackIcon.fillAmount = 1f;
        }

        if (dashIcon != null)
        {
            if (cooldownTimer > 0f)
                dashIcon.fillAmount = 1 - (cooldownTimer / dashCooldown);
            else
                dashIcon.fillAmount = 1f;
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
            return;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }

        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.maxHealth = 100f;
            playerHealth.health = 100f;

            // Update UI immediately
            if (playerHealth.HealthBar != null)
            {
                playerHealth.HealthBar.fillAmount = 1f;
            }

            Debug.Log("Player health reset to 100 on scene load.");
        }

    }
}