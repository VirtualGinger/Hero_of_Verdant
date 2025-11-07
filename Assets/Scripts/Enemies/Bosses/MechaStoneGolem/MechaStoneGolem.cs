using UnityEngine;

public class Boss_Behavior : MonoBehaviour
{
    // --- AI STATES ---
    private enum AIState { Idle, Repositioning, Attacking }
    private AIState currentState = AIState.Idle;

    // --- SETUP ---
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float verticalAlignmentTolerance = 0.5f; 

    [Header("Combat Stats")]
    public float laserAttackRange = 20f;
    public float laserCooldown = 5f;

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

    void Awake()
    {
        // --- IT'S RIGHT HERE ---
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
        // 1. Check for death
        if (_enemyHealth != null && _enemyHealth.health <= 0)
        {
            anim.SetBool("canWalk", false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        // 2. Check for Target
        if (target == null)
        {
            inRange = false;
            anim.SetBool("canWalk", false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            currentState = AIState.Idle;
            return;
        }

        // 3. Update Cooldown
        timeSinceLastLaser += Time.deltaTime;

        // 4. Run AI State Machine
        if (inRange && target != null)
        {
            UpdateAIState();
        }
    }

    void UpdateAIState()
        {
            // Always face the player and get distance
            Flip(target.transform.position.x);
            distance = Vector2.Distance(transform.position, target.transform.position);

            switch (currentState)
            {
                // --- STATE 1: IDLE ---
                case AIState.Idle:
                    anim.SetBool("canWalk", false);
                    rb.linearVelocity = Vector2.zero;

                    // Check if cooldown is ready AND player is in range
                    if (timeSinceLastLaser >= laserCooldown && distance <= laserAttackRange)
                    {
                        currentState = AIState.Repositioning;
                    }
                    break;

                // --- STATE 2: REPOSITIONING ---
                case AIState.Repositioning:
                    
                    // --- THIS IS THE FIX ---
                    // Check if player escaped WHILE we are moving
                    if (distance > laserAttackRange) 
                    {
                        currentState = AIState.Idle; // Player got away, go back to idle
                        break;
                    }
                    // --- END FIX ---

                    float yDifference = target.transform.position.y - transform.position.y;

                    if (Mathf.Abs(yDifference) > verticalAlignmentTolerance)
                    {
                        // Not aligned yet, keep moving
                        rb.linearVelocity = new Vector2(0, Mathf.Sign(yDifference) * moveSpeed);
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

                // --- STATE 3: ATTACKING ---
                case AIState.Attacking:
                    rb.linearVelocity = Vector2.zero;
                    anim.SetBool("canWalk", false);
                    // Waiting for animation event...
                    break;
            }
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
            // --- ADDED THIS LINE ---
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
        Debug.Log("--- Attack finished, returning to Idle. ---");
        currentState = AIState.Idle;
    }
}