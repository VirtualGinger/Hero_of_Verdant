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
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (laser != null)
            laserComponent = laser.GetComponent<Laser>();

        attackTimer = attackCooldown;
    }

    void Update()
    {
        // If the enemy is dead, do not run AI movement
        if (animator != null && animator.GetBool("isDead"))
            return;

        if (player == null)
            return;

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
            laserComponent.Activate(player.position);

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
            laserComponent.Deactivate();

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
}