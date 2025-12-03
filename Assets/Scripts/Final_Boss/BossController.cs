using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer sr;
    public Transform player;

    [Header("Stats")]
    public float maxHealth = 80f;
    public float health = 80f;
    public float moveSpeed = 2f;

    [Header("Ranges")]
    public float chaseRange = 6f;
    public float attack1Range = 3f;     // Laser range
    public float attack2Range = 2f;     // AOE range

    [Header("Cooldowns")]
    public float attack1Cooldown = 2f;
    public float attack2Cooldown = 5f;
    private float attack1Timer = 0f;
    private float attack2Timer = 0f;

    [Header("Damage")]
    public float laserDamage = 5f;
    public float aoeDamage = 12f;
    public LayerMask playerLayer;

    [Header("Attack Spawns")]
    public Transform laserPoint;
    public Transform aoePoint;
    public GameObject laserPrefab;
    public GameObject aoePrefab;

    private Vector2 moveDir;
    private bool isDead = false;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        health = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        if (attack1Timer > 0) attack1Timer -= Time.deltaTime;
        if (attack2Timer > 0) attack2Timer -= Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        // AOE Attack (highest priority)
        if (dist <= attack2Range && attack2Timer <= 0)
        {
            DoAOEAttack();
            return;
        }

        // Laser Attack
        if (dist <= attack1Range && attack1Timer <= 0)
        {
            DoLaserAttack();
            return;
        }

        // Chase if in range
        if (dist <= chaseRange)
            MoveTowardPlayer();
        else
            StopMoving();
    }

    private void MoveTowardPlayer()
    {
        moveDir = (player.position - transform.position).normalized;

        // Flip sprite for right-facing-only animations
        sr.flipX = moveDir.x < 0;

        animator.SetBool("IsWalking", true);
    }

    private void StopMoving()
    {
        moveDir = Vector2.zero;
        animator.SetBool("IsWalking", false);
    }

    void FixedUpdate()
    {
        if (!isDead)
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    // ----------------------------
    //         ATTACK 1 (LASER)
    // ----------------------------
    private void DoLaserAttack()
    {
        attack1Timer = attack1Cooldown;
        animator.SetTrigger("Attack1");  // Animation event will call SpawnLaser()
        StopMoving();
    }

    public void SpawnLaser() // <-- MUST be inside class!
    {
        Instantiate(laserPrefab, laserPoint.position, Quaternion.identity);

        // Damage player if close
        Collider2D hit = Physics2D.OverlapCircle(laserPoint.position, 0.75f, playerLayer);
        if (hit != null)
        {
            PlayerHealth hp = hit.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(laserDamage);
        }
    }

    // ----------------------------
    //         ATTACK 2 (AOE)
    // ----------------------------
    private void DoAOEAttack()
    {
        attack2Timer = attack2Cooldown;
        animator.SetTrigger("Attack2");  // Animation event will call SpawnAOE()
        StopMoving();
    }

    public void SpawnAOE() // <-- MUST be inside class!
    {
        Instantiate(aoePrefab, aoePoint.position, Quaternion.identity);

        Collider2D hit = Physics2D.OverlapCircle(aoePoint.position, 1f, playerLayer);
        if (hit != null)
        {
            PlayerHealth hp = hit.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(aoeDamage);
        }
    }

    // ----------------------------
    //            DAMAGE
    // ----------------------------
    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        health -= dmg;
        animator.SetTrigger("Hit");

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        if (laserPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(laserPoint.position, 0.75f);
        }

        if (aoePoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(aoePoint.position, 1f);
        }
    }
}