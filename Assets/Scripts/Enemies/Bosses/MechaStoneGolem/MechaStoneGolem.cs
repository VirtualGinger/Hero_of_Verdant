using UnityEngine;

public class Boss_Behavior : MonoBehaviour
{
    // --- From your script ---
    [Header("Detection")]
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float verticalTolerance = 0.5f;

    [Header("Movement")]
    public float moveSpeed;
    
    [Header("Combat Stats")]
    public float meleeAttackRange; // Renamed from attackDistance
    public float laserAttackRange = 10f;
    
    public float laserCooldown = 8f;
    public float meleeCooldown = 3f;
    public float blockCooldown = 10f;
    [Range(0, 1)]
    public float blockChance = 0.3f; // 30% chance to block

    // --- Private Variables ---
    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool inRange;
    private bool isFacingRight = true;
    private float originalScaleMagnitudeX;
    private EnemyHealth _enemyHealth;

    // New Cooldown Timers
    private float timeSinceLastLaser;
    private float timeSinceLastMelee;
    private float timeSinceLastBlock;

    // State
    private bool isAttacking = false;
    private bool isBlocking = false;


    void Awake()
    {
        anim = GetComponent<Animator>();
        originalScaleMagnitudeX = Mathf.Abs(transform.localScale.x);
        _enemyHealth = GetComponent<EnemyHealth>();

        // Start with cooldowns ready
        timeSinceLastLaser = laserCooldown;
        timeSinceLastMelee = meleeCooldown;
        timeSinceLastBlock = blockCooldown;

        // --- DEBUG ---
        Debug.Log("BOSS AWAKE: Animator is " + (anim != null ? "FOUND" : "MISSING"));
        Debug.Log("BOSS AWAKE: EnemyHealth is " + (_enemyHealth != null ? "FOUND" : "MISSING"));
    }

    void Update()
    {
        // 1. Check for death
        if (_enemyHealth != null && _enemyHealth.health <= 0)
        {
            anim.SetBool("canWalk", false);
            return;
        }

        // 2. Check for Target
        if (target == null)
        {
            // This is the normal "idle" state
            inRange = false;
            anim.SetBool("canWalk", false);
            return; // No target, do nothing
        }

        // 3. Update Cooldowns
        timeSinceLastLaser += Time.deltaTime;
        timeSinceLastMelee += Time.deltaTime;
        timeSinceLastBlock += Time.deltaTime;

        // 4. Check Line of Sight
        if (inRange)
        {
            HandleLineOfSight();
        }

        // 5. Run AI Logic (if we have a target and aren't busy)
        if (inRange && !isAttacking && !isBlocking)
        {
            BossLogic();
        }
        else if (isAttacking)
        {
            // --- DEBUG ---
            // If you see this message spamming, your Animation Event is broken
            // Debug.Log("Boss is busy ATTACKING...");
        }
        else if (isBlocking)
        {
            // --- DEBUG ---
            // If you see this message spamming, your Animation Event is broken
            // Debug.Log("Boss is busy BLOCKING...");
        }
    }

    void HandleLineOfSight()
    {
        if (target == null) return;

        float targetX = target.transform.position.x;
        Vector2 direction = (targetX < transform.position.x) ? Vector2.left : Vector2.right;

        Flip(targetX);

        hit = Physics2D.Raycast(rayCast.position, direction, rayCastLength, raycastMask);
        RaycastDebugger(direction);

        if (hit.collider == null)
        {
            // --- DEBUG ---
            Debug.Log("LINE OF SIGHT: FAILED. Losing target.");
            inRange = false;
            target = null;
            anim.SetBool("canWalk", false);
        }
        else
        {
            // --- DEBUG ---
            // This should spam every frame you are in range and in sight
            // Debug.Log("LINE OF SIGHT: OK. Player detected: " + hit.collider.name);
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            // --- DEBUG ---
            Debug.Log("!!!!!!!!!!!!!!! PLAYER ENTERED RANGE !!!!!!!!!!!!!!!");
            target = trig.gameObject;
            inRange = true;
        }
    }

    void BossLogic()
    {
        if (target == null) return;

        distance = Vector2.Distance(transform.position, target.transform.position);

        // --- AI Decision Making ---

        // 1. Laser Attack (Long range)
        if (distance <= laserAttackRange && distance > meleeAttackRange && timeSinceLastLaser >= laserCooldown)
        {
            // --- DEBUG ---
            Debug.Log("BOSS LOGIC: Performing LASER attack");
            PerformLaserAttack();
        }
        // 2. Melee Attack (Short range)
        else if (distance <= meleeAttackRange && timeSinceLastMelee >= meleeCooldown)
        {
            // --- DEBUG ---
            Debug.Log("BOSS LOGIC: Performing MELEE attack");
            PerformMeleeAttack();
        }
        // 3. Move (If out of melee range and not lasering)
        else if (distance > meleeAttackRange)
        {
            // --- DEBUG ---
            // Debug.Log("BOSS LOGIC: Moving to player");
            Move();
        }
        // 4. Idle (in melee range but on cooldown)
        else
        {
            anim.SetBool("canWalk", false);
        }
    }

    void Move()
    {
        anim.SetBool("canWalk", true);
        
        float yDifference = Mathf.Abs(target.transform.position.y - transform.position.y);
        Vector2 targetPosition;

        if (yDifference > verticalTolerance)
        {
            targetPosition = new Vector2(transform.position.x, target.transform.position.y);
        }
        else
        {
            targetPosition = new Vector2(target.transform.position.x, transform.position.y);
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void PerformLaserAttack()
    {
        isAttacking = true;
        timeSinceLastLaser = 0f;
        anim.SetBool("canWalk", false);
        anim.SetTrigger("doLaser");
    }

    void PerformMeleeAttack()
    {
        isAttacking = true;
        timeSinceLastMelee = 0f;
        anim.SetBool("canWalk", false);
        anim.SetTrigger("doMelee");
    }

    void PerformBlock()
    {
        isBlocking = true;
        timeSinceLastBlock = 0f;
        anim.SetBool("canWalk", false);
        anim.SetTrigger("doBlock");
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

    // --- PUBLIC FUNCTIONS ---

    public void OnTakeDamage()
    {
        if (isBlocking)
        {
            Debug.Log("BOSS: Attack Blocked!");
            return;
        }

        if (timeSinceLastBlock >= blockCooldown && Random.value <= blockChance)
        {
            // --- DEBUG ---
            Debug.Log("BOSS LOGIC: Decided to BLOCK");
            PerformBlock();
        }
        else
        {
            Debug.Log("BOSS: Took damage!");
            // anim.SetTrigger("doHurt"); 
        }
    }


    // --- ANIMATION EVENT FUNCTIONS ---

    public void ANIM_EVENT_FireLaser()
    {
        // --- DEBUG ---
        Debug.Log("!!! ANIM_EVENT_FireLaser CALLED !!!");
        Debug.Log("FIRING LASER!");
    }

    public void ANIM_EVENT_StartBlock()
    {
        // --- DEBUG ---
        Debug.Log("!!! ANIM_EVENT_StartBlock CALLED !!!");
        isBlocking = true;
    }

    public void ANIM_EVENT_EndBlock()
    {
        // --- DEBUG ---
        Debug.Log("!!! ANIM_EVENT_EndBlock CALLED !!!");
        isBlocking = false;
    }

    public void ANIM_EVENT_AttackFinished()
    {
        // --- DEBUG ---
        Debug.Log("!!! ANIM_EVENT_AttackFinished CALLED !!!");
        isAttacking = false;
    }

    public void ResetWeaponHitbox()
    {
        Damage weaponDamage = GetComponentInChildren<Damage>();
        if (weaponDamage != null)
        {
            weaponDamage.hasHit = false;
        }
    }


    void RaycastDebugger(Vector2 direction)
    {
        if (target == null) return;
        
        Color rayColor = (distance <= meleeAttackRange) ? Color.green : Color.red;
        Debug.DrawRay(rayCast.position, direction * rayCastLength, rayColor);
    }
}