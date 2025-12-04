using UnityEngine;
using System.Collections; // Needed for Coroutines

public class crab_behavior : MonoBehaviour
{
    // --- EXPLOSION PARAMETERS ---
    public float explosionRadius = 3f; // The actual damage radius when exploding
    public int explosionDamage = 20; // Damage dealt on explosion
    public float attackDistance = 1.5f; // The range at which the enemy triggers the explosion
    private bool isExploding = false; // Prevents re-triggering the explosion

    // --- ORIGINAL PARAMETERS ---
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float moveSpeed;
    public float verticalTolerance = 0.5f;
    public bool useDiagonalMovement = false;

    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool inRange;
    private bool isFacingRight = true;
    private float originalScaleMagnitudeX;
    private EnemyHealth _enemyHealth;


    void Awake()
    {
        anim = GetComponent<Animator>();
        originalScaleMagnitudeX = Mathf.Abs(transform.localScale.x);
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        // If dead or already exploding, stop all movement and logic
        if (_enemyHealth.health <= 0 || isExploding)
        {
            // Ensure movement is stopped
            anim.SetBool("canWalk", false);
            return;
        }

        if (inRange && target != null)
        {
            // Target tracking and Raycast checks (same as before)
            float targetX = target.transform.position.x;
            Vector2 direction = (targetX < transform.position.x) ? Vector2.left : Vector2.right;

            Flip(targetX);

            hit = Physics2D.Raycast(rayCast.position, direction, rayCastLength, raycastMask);
            RaycastDebugger();

            if (hit.collider != null)
            {
                EnemyLogic(); // Check for explosion
            }
            else
            {
                inRange = false;
            }
        }
        else
        {
            inRange = false;
        }

        if (inRange == false)
        {
            anim.SetBool("canWalk", false);
            target = null;
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            target = trig.gameObject;
            inRange = true;
        }
    }

    void EnemyLogic()
    {
        if (target == null) return;

        distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackDistance)
        {
            Move(); // Keep moving towards the player
        }
        else // Player is within attackDistance
        {
            // Trigger the explosion sequence
            ExplodeSequence();
        }
    }

    void Move()
    {
        anim.SetBool("canWalk", true);
        if (target == null) return;

        // The diagonal/vertical movement logic is kept here
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Explode_State_Name")) // Assuming you'll have an Explode animation state
        {
            if (useDiagonalMovement)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
                return;
            }

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
    }

    // --- NEW EXPLOSION LOGIC ---
    void ExplodeSequence()
    {
        if (isExploding) return;

        isExploding = true;
        anim.SetBool("canWalk", false);

        // Trigger the explosion animation immediately
        anim.SetTrigger("Explode");

        // The actual damage calculation will be done by an Animation Event
        // (See instructions below)
    }

    // This method MUST be called as an Animation Event from the explosion animation clip.
    // It is public so the animation system can find it.
    public void DealExplosionDamage()
    {
        // Log 1: Confirm this function is firing when the animation event occurs.
        Debug.Log("DEALING EXPLOSION DAMAGE: Function Fired at position " + transform.position);

        // 1. Find all colliders within the explosion radius (checking ALL layers by default)
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        // Log 2: Check how many objects were actually detected in the radius.
        Debug.Log($"OverlapCircle found {hitObjects.Length} objects with radius {explosionRadius}.");

        // 2. Iterate through all hit objects
        bool playerHit = false;

        foreach (Collider2D collider in hitObjects)
        {
            // Log 3: Check what objects were found.
            Debug.Log($"Found Collider: {collider.gameObject.name} (Tag: {collider.tag})");

            if (collider.CompareTag("Player"))
            {
                playerHit = true;
                // 3. Apply damage to the player
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(explosionDamage);
                    Debug.Log($"SUCCESS: Explosion dealt {explosionDamage} damage to Player!");
                    break; // Stop searching once the player is hit
                }
                else
                {
                    // Log 4: If this runs, the Player tag is correct, but the PlayerHealth script is missing or misspelled.
                    Debug.LogError("Player found but PlayerHealth component is missing!");
                }
            }
        }

        // Log 5: Final check
        if (!playerHit)
        {
            Debug.LogWarning("Player was not detected within the explosion radius!");
        }

        // 4. Destroy the enemy object after damage is dealt
        Destroy(gameObject, 0.1f);
    }
    // --- END NEW EXPLOSION LOGIC ---


    // The following methods are largely unchanged or removed/simplified
    // Cooldown and Attack are removed/replaced.
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

    void RaycastDebugger()
    {
        if (target == null) return;
        Vector2 direction = (target.transform.position.x < transform.position.x) ? Vector2.left : Vector2.right;

        // Change ray color to white when preparing to explode (in range)
        if (distance <= attackDistance)
        {
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.white);
        }
        else
        {
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.red);
        }
        // Optional: Draw the explosion radius
        // Debug.DrawRay(transform.position, Vector3.up * explosionRadius, Color.yellow); 
    }

    // Unused/Obsolete methods removed: Cooldown, StopAttack, TriggerCooling, ResetWeaponHitbox
}