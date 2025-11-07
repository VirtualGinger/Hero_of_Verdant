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

    [Header("Blocking")]
    public float blockHoldTime = 1.5f; // Configurable time to hold the block

    // --- Private Variables ---
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool inRange;
    private bool isFacingRight = true;
    private float originalScaleMagnitudeX;
    private EnemyHealth _enemyHealth;
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
                
                // --- (This "player escaped" fix is still here) ---
                if (target == null || distance > laserAttackRange) 
                {
                    currentState = AIState.Idle; // Player got away, go back to idle
                    anim.SetBool("canWalk", false); // --- NEW: Stop anim
                    rb.linearVelocity = Vector2.zero; // --- NEW: Stop moving
                    break;
                }
                // --- END FIX ---

                // Get difference on BOTH axes
                float yDifference = target.transform.position.y - transform.position.y;
                float xDifference = target.transform.position.x - transform.position.x; // --- NEW ---

                if (Mathf.Abs(yDifference) > verticalAlignmentTolerance)
                {
                    // --- MODIFIED BLOCK ---
                    // Y is not aligned, so we move.
                    
                    // Get the direction for both axes
                    float yDir = Mathf.Sign(yDifference);
                    float xDir = Mathf.Sign(xDifference);
                    
                    // Create a combined direction vector (e.g., [1, 1] or [-1, 1])
                    Vector2 moveDirection = new Vector2(xDir, yDir);
                    
                    // Normalize it and apply speed
                    // .normalized ensures the boss doesn't move faster on diagonals
                    rb.linearVelocity = moveDirection.normalized * moveSpeed;
                    
                    // This line is in your code, but as you said,
                    // you have no "walk" animation. It's safe to
                    // keep or remove.
                    anim.SetBool("canWalk", true); 
                }
                else
                {
                    // --- (This block is unchanged) ---
                    // We are aligned AND player is still in range. ATTACK.
                    rb.linearVelocity = Vector2.zero;
                    anim.SetBool("canWalk", false);
                    PerformLaserAttack();
                }
                break;

            // --- STATE 3: ATTACKING ---
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