using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking, Hit, Dead }
    private State currentState = State.Idle;

    [Header("Setup")]
    public float moveSpeed = 2f;
    public float attackRange = 1.2f;
    public float attackCooldown = 2f;
    public LayerMask playerLayer;

    [Header("Animation")]
    private Animator anim;
    private Rigidbody2D rb;

    private Transform player;
    private float lastAttackTime = 0f;
    private bool isFacingRight = true;
    private float originalScaleX;

    [Header("Health")]
    public int health = 30;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        originalScaleX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Chase
        if (distance > attackRange)
        {
            if (currentState != State.Hit)
                currentState = State.Chasing;
        }
        // Attack
        else
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                currentState = State.Attacking;
                rb.linearVelocity = Vector2.zero; 
                ChooseAttack();
            }
        }

        HandleState();
        FlipSprite();
    }

    void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isWalking", false);
                break;

            case State.Chasing:
                ChasePlayer();
                break;

            case State.Attacking:
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isWalking", false);
                break;

            case State.Hit:
                rb.linearVelocity = Vector2.zero;
                break;

            case State.Dead:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
        anim.SetBool("isWalking", true);
    }

    void ChooseAttack()
    {
        lastAttackTime = Time.time;

        int attackPick = Random.Range(1, 3);

        if (attackPick == 1)
            anim.SetTrigger("Attack1");
        else
            anim.SetTrigger("Attack2");
    }

    // Called from ANIMATION EVENT
    public void ANIM_EVENT_DealDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null)
        {
            PlayerHealth playerHP = hit.GetComponent<PlayerHealth>();
            if (playerHP != null)
                playerHP.TakeDamage(10);
        }
    }

    void FlipSprite()
    {
        if (player == null) return;

        bool shouldFaceRight = player.position.x > transform.position.x;

        if (shouldFaceRight != isFacingRight)
        {
            isFacingRight = shouldFaceRight;
            transform.localScale = new Vector3(
                isFacingRight ? originalScaleX : -originalScaleX,
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    public void TakeDamage(int dmg)
    {
        if (currentState == State.Dead) return;

        health -= dmg;

        anim.SetTrigger("Hit");
        currentState = State.Hit;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        currentState = State.Dead;
        anim.SetTrigger("Death");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f); // optional delay
    }

    // Called by Hit animation event to return to AI
    public void ANIM_EVENT_HitFinished()
    {
        currentState = State.Idle;
    }
}