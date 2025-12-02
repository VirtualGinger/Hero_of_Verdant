using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 6f;
    public float attackRange = 1.5f;
    public float moveSpeed = 3f;
    public float attackCooldown = 1f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform player;

    private bool isChasing = false;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    private Vector2 movement;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        // Player enters chase range
        if (distance <= chaseRange)
            isChasing = true;

        // If not yet chasing, stay still
        if (!isChasing)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        // Determine left/right facing
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // face left
        else
            transform.localScale = new Vector3(1, 1, 1); // face right

        // If within attack distance
        if (distance <= attackRange)
        {
            movement = Vector2.zero;
            animator.SetBool("isMoving", false);

            // Attack only when cooldown is ready
            if (attackTimer <= 0f)
            {
                isAttacking = true;
                animator.SetTrigger("attack");
                attackTimer = attackCooldown;
            }

            return; // do not move while attacking
        }

        // Chase player (not attacking)
        isAttacking = false;
        Vector2 direction = (player.position - transform.position).normalized;
        movement = direction;

        animator.SetBool("isMoving", true);
    }

    void FixedUpdate()
    {
        if (isChasing && !isAttacking)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Optional: visualize ranges
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
