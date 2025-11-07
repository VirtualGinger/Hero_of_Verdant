using UnityEngine;

public class BossHealth : EnemyHealth
{
    private Boss_Behavior bossBehavior;
    private Animator bossAnimator; 

    [Header("Blocking Logic")]
    [Tooltip("How many hits it takes to trigger a block.")]
    public int hitsToBlock = 3; 
    private int hitCounter = 0;

    // --- NEW ---
    [Header("Death")]
    [Tooltip("How long to wait after death before disappearing.")]
    [SerializeField] private float deathDisappearDelay = 3f;
    private bool isDead = false; // Flag to stop logic from running
    // --- END NEW ---

    
    new void Start()
    {
        bossAnimator = GetComponent<Animator>(); 
        currentHealth = health;
        bossBehavior = GetComponent<Boss_Behavior>();
        hitCounter = 0;
        
        // --- NEW ---
        isDead = false; // Make sure we're not dead on start
        // --- END NEW ---
    }

    new void Update()
    {
        // --- NEW ---
        // If the boss is dead, stop running all update logic
        if (isDead)
        {
            return;
        }
        // --- END NEW ---


        // --- Original Hit Logic ---
        if (health < currentHealth)
        {
            currentHealth = health;
            bossAnimator.SetTrigger("Attacked");

            hitCounter++;
            if (hitCounter >= hitsToBlock)
            {
                hitCounter = 0; 
                if (bossBehavior != null)
                {
                    bossBehavior.OnHit();
                }
            }
        }
        // --- End Original Hit Logic ---


        // --- MODIFIED Death Logic ---
        if (health <= 0)
        {
            isDead = true; // Set the flag so this block only runs ONCE
            
            bossAnimator.SetBool("isDead", true);
            Debug.Log("Enemy Dead");

            // --- ADD THIS LINE ---
            // Tell Unity to destroy this boss GameObject after 'deathDisappearDelay' seconds
            Destroy(gameObject, deathDisappearDelay);
        }
        // --- END MODIFIED ---
    }
}