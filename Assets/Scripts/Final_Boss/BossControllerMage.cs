using UnityEngine;
using System.Collections;

public class BossControllerMage : MonoBehaviour
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

    public float health
    {
        get { return currentHealth; }
        set
        {
            int damageTaken = Mathf.CeilToInt(currentHealth - value);

            if (damageTaken > 0)
            {
                TakeDamage(damageTaken);
            }
        }
    }

    [Header("Animation")]
    private Animator animator;
    private SpriteRenderer sprite;
    private Collider2D bossCollider;

    [Header("Laser Settings")]
    public GameObject laser;
    public Transform laserSpawnPoint;
    public float laserDuration = 0.4f;

    private Laser laserComponent;

    private bool isFacingRight = true;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (laser != null)
        {
            laserComponent = laser.GetComponent<Laser>();
        }

        attackTimer = attackCooldown;
    }

    void Update()
    {
     
        if(currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            Destroy(gameObject);
        }
        if (currentHealth <= 0)
        {
            if (animator != null && animator.GetBool("isDead") == false)
            {
                Die();
            }
            return;
        }

        if (player == null) return;

        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        inRange = distanceToPlayer <= chaseRange;

        if (inRange)
        {
            FacePlayer();

            if (distanceToPlayer > attackRange)
            {
                MoveTowardPlayer();
                StopAttack();
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

        if (laserComponent != null && player != null)
        {
            laserComponent.Activate(player.position);
        }

        attackCooling = true;
        attackTimer = attackCooldown;
    }

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

        if (laserComponent != null)
        {
            laserComponent.Deactivate();
        }

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
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth > 0 && animator != null)
        {
            animator.SetTrigger("isHit");
        }

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (animator != null && animator.GetBool("isDead") == false)
        {
            // 1. Stop all combat/movement related functions
            StopAttack();

            // 2. Trigger the death animation
            animator.SetBool("isDead", true);

            // 3. Disable the main collider.
            if (bossCollider != null)
            {
                bossCollider.enabled = false;
            }
            else
            {
                // Correct way to handle potential null result when assigning a value type property
                Collider2D tempCollider = GetComponent<Collider2D>();
                if (tempCollider != null)
                {
                    tempCollider.enabled = false;
                }
            }

            // 4. Schedule destruction of the object after the animation duration (1 second)
            Destroy(gameObject, 1f);

            // Explicitly disable this MonoBehaviour instance to stop Update()
            //this.enabled = false;

            Debug.Log("Boss Dead - Animation Triggered and Destruction Scheduled.");
        }
    }
}