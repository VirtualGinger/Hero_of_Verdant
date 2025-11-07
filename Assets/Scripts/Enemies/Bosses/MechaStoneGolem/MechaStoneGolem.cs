using UnityEngine;

public class Boss_Behavior : MonoBehaviour
{
    // --- AI STATES ---
    private enum AIState { Idle, Repositioning, Attacking, Blocking }
    private AIState currentState = AIState.Idle;

    // --- SETUP ---
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float verticalAlignmentTolerance = 0.5f;

    [Header("Combat Stats")]
    public float laserAttackRange = 20f;
    public float laserCooldown = 5f;
    // --- NEW ---
    [Tooltip("How much damage the laser deals.")]
    public int laserDamage = 10; // Changed to int for your Health script
    [Tooltip("The empty GameObject where the laser fires from.")]
    public Transform laserOriginPoint; 
    [Tooltip("Set this to the layer your Player is on.")]
    public LayerMask playerLayer; 
    // --- END NEW ---

    [Header("Blocking")]
    public float blockHoldTime = 1.5f; // Configurable time to hold the block

    // --- Private Variables ---
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool inRange;
    private bool isFacingRight = true;
    private float originalScaleMagnitudeX;
    private EnemyHealth _enemyHealth; // Make sure this is BossHealth if you're using that
    private Rigidbody2D rb;
    private float timeSinceLastLaser;
    private float blockTimer;

    void Awake()
    {
        Debug.Log("!!!!!!!!!!!!!!! BOSS SCRIPT IS AWAKE !!!!!!!!!!!!!!!");

        anim = GetComponent<Animator>();
        originalScaleMagnitudeX = Mathf.Abs(transform.localScale.x);
        _enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();

        currentState = AIState.Idle;
        timeSinceLastLaser = laserCooldown;

        if (rb == null) { Debug.LogError("BOSS IS MISSING RIGIDBODY2D!"); }
        
        // --- NEW ---
        if (laserOriginPoint == null) 
        {
            Debug.LogError("BOSS IS MISSING 'laserOriginPoint' TRANSFORM!");
        }
        // --- END NEW ---
    }

    void Update()
    {
        if (_enemyHealth != null && _enemyHealth.health <= 0)
        {
            anim.SetBool("canWalk", false);
            anim.SetBool("isBlocking", false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }


        if (target == null)
        {
            inRange = false;

            if (currentState != AIState.Blocking)
            {
                anim.SetBool("canWalk", false);
                if (rb != null) rb.linearVelocity = Vector2.zero;
                currentState = AIState.Idle;
            }
        }

        timeSinceLastLaser += Time.deltaTime;

        UpdateAIState(); 
    }

    void UpdateAIState()
    {
        if (target != null)
        {
            Flip(target.transform.position.x);
            distance = Vector2.Distance(transform.position, target.transform.position);
        }

        switch (currentState)
        {
            case AIState.Idle:
                anim.SetBool("canWalk", false);
                rb.linearVelocity = Vector2.zero;

                if (target != null && inRange && timeSinceLastLaser >= laserCooldown && distance <= laserAttackRange)
                {
                    currentState = AIState.Repositioning;
                }
                break;

            case AIState.Repositioning:
                
                if (target == null || distance > laserAttackRange) 
                {
                    currentState = AIState.Idle;
                    anim.SetBool("canWalk", false);
                    rb.linearVelocity = Vector2.zero;
                    break;
                }

                // Get difference on BOTH axes
                float yDifference = target.transform.position.y - transform.position.y;
                float xDifference = target.transform.position.x - transform.position.x;

                if (Mathf.Abs(yDifference) > verticalAlignmentTolerance)
                {
                    // Y is not aligned, so we move.
                    float yDir = Mathf.Sign(yDifference);
                    float xDir = Mathf.Sign(xDifference);
                    
                    Vector2 moveDirection = new Vector2(xDir, yDir);
                    
                    rb.linearVelocity = moveDirection.normalized * moveSpeed;
                    
                    anim.SetBool("canWalk", true); 
                }
                else
                {
                    // We are aligned AND player is still in range. ATTACK.
                    rb.linearVelocity = Vector2.zero;
                    anim.SetBool("canWalk", false);
                    PerformLaserAttack();
                }
                break;

            case AIState.Attacking:
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("canWalk", false);
                
                if (target == null)
                {
                    currentState = AIState.Idle;
                    break;
                }
                break;

            case AIState.Blocking:
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("canWalk", false);
                
                blockTimer += Time.deltaTime;
                
                if (blockTimer >= blockHoldTime)
                {
                    anim.SetBool("isBlocking", false);
                    currentState = AIState.Idle;
                }
                break;
        }
    }

    public void OnHit()
    {
        if (currentState == AIState.Blocking || (_enemyHealth != null && _enemyHealth.health <= 0))
        {
            return;
        }

        Debug.Log("--- BOSS HIT, ENTERING BLOCK STATE ---");
        
        currentState = AIState.Blocking;
        blockTimer = 0f;

        if (rb != null) rb.linearVelocity = Vector2.zero;
        anim.SetBool("canWalk", false);

        anim.SetBool("isBlocking", true); 
    }

    void PerformLaserAttack()
    {
        currentState = AIState.Attacking;
        timeSinceLastLaser = 0f;
        anim.SetTrigger("doLaser");
    }

    // --- NEW FUNCTION ---
    /// <summary>
    /// This function is called by an animation event.
    /// It fires a raycast to damage the player.
    /// </summary>
    public void ANIM_EVENT_FireLaser()
    {
        if (target == null) return; // Player is gone

        // Ensure we're aiming at the player's current position
        Vector2 direction = (target.transform.position - laserOriginPoint.position).normalized;

        // Draw a debug ray so you can see it in the Scene view
        Debug.DrawRay(laserOriginPoint.position, direction * laserAttackRange, Color.red, 2f);

        // Fire the raycast
        RaycastHit2D hit = Physics2D.Raycast(
            laserOriginPoint.position, 
            direction, 
            laserAttackRange, 
            playerLayer // This makes sure it ONLY hits the player
        );

        if (hit.collider != null)
        {
            // We hit something on the player layer!
            // Try to get the Health script from it.
            Health playerHealth = hit.collider.GetComponent<Health>();
            if (playerHealth != null)
            {
                // We found it. Deal damage!
                // We pass the damage (as an int) and this boss's GameObject as the "sender"
                playerHealth.TakeDamage(laserDamage, this.gameObject);
            }
        }
    }
    // --- END NEW FUNCTION ---

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            Debug.Log("!!!!!!!!!!!!!! PLAYER ENTERED TRIGGER !!!!!!!!!!!!!!");
            target = trig.gameObject;
            inRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            Debug.Log("!!!!!!!!!!!!!! PLAYER EXITED TRIGGER !!!!!!!!!!!!!!");
            target = null;
            inRange = false;
        }
    }

    void Flip(float targetX)
    {
        if (targetX > transform.position.x && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(originalScaleMagnitudeX, transform.localScale.y, transform.localScale.z);
        }
        else if (targetX < transform.position.x && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-originalScaleMagnitudeX, transform.localScale.y, transform.localScale.z);
        }
    }


    public void ANIM_EVENT_AttackFinished()
    {
        if (currentState == AIState.Blocking)
        {
            Debug.Log("--- Attack finished, BUT we are blocking. ---");
            return;
        }

        Debug.Log("--- Attack finished, returning to Idle. ---");
        currentState = AIState.Idle;
    }
}