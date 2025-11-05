using UnityEngine;
using UnityEngine.AI; // Required for movement

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BossAI : MonoBehaviour
{
    // --- DRAG-AND-DROP IN INSPECTOR ---
    [Header("Required Setup")]
    public Transform playerTarget; // Drag the Player object here
    
    [Header("Attack Settings")]
    public float laserAttackRange = 15f;
    public float laserCooldown = 8f;
    
    public float blockChance = 0.3f; // 30% chance to block when player attacks
    public float blockCooldown = 10f;

    // --- Private Variables ---
    private NavMeshAgent agent;
    private Animator animator;
    private float timeSinceLastLaser = 0f;
    private float timeSinceLastBlock = 0f;
    private bool isBlocking = false;

    void Start()
    {
        // Get the components automatically
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (playerTarget == null)
        {
            Debug.LogError("BOSS AI: Player Target is not set!");
        }
    }

    void Update()
    {
        // If we don't have a target or are blocking, stop everything
        if (playerTarget == null || isBlocking)
        {
            agent.isStopped = true;
            animator.SetBool("isMoving", false);
            return;
        }

        // --- Update Cooldowns ---
        timeSinceLastLaser += Time.deltaTime;
        timeSinceLastBlock += Time.deltaTime;

        // --- Core AI Logic ---
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // 1. A-I: ATTACK (Laser)
        if (distanceToPlayer <= laserAttackRange && timeSinceLastLaser >= laserCooldown)
        {
            PerformLaserAttack();
        }
        // 2. A-I: CHASE (Movement)
        else
        {
            // Move towards the player
            agent.SetDestination(playerTarget.position);
            agent.isStopped = false;
        }
        
        // --- Update Animator ---
        // Update the isMoving bool based on if the agent is actually moving
        animator.SetBool("isMoving", agent.velocity.magnitude > 0.1f);
    }

    private void PerformLaserAttack()
    {
        timeSinceLastLaser = 0f; // Reset cooldown
        agent.isStopped = true; // Stop moving to attack
        
        // Face the player
        transform.LookAt(playerTarget.position); 

        // Trigger the animation in the Animator
        animator.SetTrigger("doLaser");
    }

    // --- ANIMATION EVENT FUNCTION ---
    // This function MUST be called by an Animation Event on your 'LaserAttack' animation
    // at the exact frame the laser should fire.
    public void ANIM_EVENT_FireLaser()
    {
        //
        // TODO: PUT YOUR LASER LOGIC HERE
        // (e.g., Instantiate laser prefab, Raycast for damage, etc.)
        //
        Debug.Log("BOSS: Firing Laser!");
    }

    // --- Public function to be called by the Player's attack script ---
    // Example: When your player's sword hits the boss, it calls this function.
    public void OnTakeDamage()
    {
        // Check if we can block
        if (timeSinceLastBlock >= blockCooldown && Random.value <= blockChance)
        {
            PerformBlock();
        }
        else
        {
            //
            // TODO: PUT YOUR "TAKE DAMAGE" LOGIC HERE
            //
            Debug.Log("BOSS: Took damage!");
        }
    }
    
    private void PerformBlock()
    {
        timeSinceLastBlock = 0f; // Reset cooldown
        agent.isStopped = true;
        animator.SetTrigger("doBlock");
    }

    // --- ANIMATION EVENT FUNCTION ---
    // Call this from an event at the START of your 'Block' animation
    public void ANIM_EVENT_StartBlock()
    {
        isBlocking = true;
        //
        // TODO: Enable your block hitbox or set a flag
        //
        Debug.Log("BOSS: Blocking enabled.");
    }

    // --- ANIMATION EVENT FUNCTION ---
    // Call this from an event at the END of your 'Block' animation
    public void ANIM_EVENT_EndBlock()
    {
        isBlocking = false;
        agent.isStopped = false; // Resume chasing
        Debug.Log("BOSS: Blocking finished.");
    }
}