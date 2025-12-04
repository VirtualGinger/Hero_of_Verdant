using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Movement & Detection")]
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public float attackRange = 3f;
    public float verticalTolerance = 0.5f;
    

    private Transform player;
    private float distanceToPlayer;
    private bool inRange;
    private bool attackCooling;
    private float attackTimer;
    public float attackCooldown = 2f;

    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Animation")]
    private Animator animator;
    private SpriteRenderer sprite;

    [Header("Laser Settings")]
    public GameObject laser;              // laser child object
    public Transform laserSpawnPoint;     // where the beam starts
    public float laserDuration = 0.4f;    // no longer used

    private bool isFacingRight = true;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // ❌ Removed: laser.SetActive(false);
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (currentHealth <= 0) return;
        if (player == null) return;

        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        inRange = distanceToPlayer <= chaseRange;

        if (inRange)
        {
            FacePlayer();

            if (distanceToPlayer > attackRange)
            {
                MoveTowardPlayer();
                StopAttack(); // DOES NOT disable laser now
            }
            else if (!attackCooling)
            {
                Attack();
            }
        }
        else
        {
            Idle();
        }

        if (attackCooling)
            Cooldown();
    }

    void MoveTowardPlayer()
    {
        animator.SetBool("isMoving", true);

        float yDiff = Mathf.Abs(player.position.y - transform.position.y);

        Vector2 targetPos = (yDiff > verticalTolerance) ?
                            new Vector2(transform.position.x, player.position.y) :
                            new Vector2(player.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    void Idle()
    {
        animator.SetBool("isMoving", false);
        StopAttack();
    }

    void Attack()
    {
        animator.SetBool("isMoving", false);
        animator.SetTrigger("Attack1");

        // ❌ Removed: laser.SetActive(true)
        // ❌ Removed: manually playing LaserAnimation
        // ❌ Removed: DisableLaserAfterTime

        attackCooling = true;
        attackTimer = attackCooldown;
    }

    // ❌ Completely removed DisableLaserAfterTime coroutine

    void Cooldown()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
            attackCooling = false;
    }

    void StopAttack()
    {
        attackCooling = false;
        animator.ResetTrigger("Attack1");
        animator.SetBool("isMoving", false);

        // ❌ Removed: laser.SetActive(false)

        attackTimer = attackCooldown;
    }

    void FacePlayer()
    {
        if (player == null) return;

        bool shouldFaceRight = player.position.x > transform.position.x;

        if (shouldFaceRight != isFacingRight)
        {
            isFacingRight = shouldFaceRight;

            sprite.flipX = !isFacingRight;

            if (laserSpawnPoint != null)
            {
                Vector3 scale = laserSpawnPoint.localScale;
                scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                laserSpawnPoint.localScale = scale;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("isHit");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        animator.SetBool("isDead", true);
        StopAttack();
        Destroy(gameObject, 1f);
    }
}
