using UnityEngine;

// Notice it inherits from EnemyHealth, not MonoBehaviour
public class BossHealth : EnemyHealth
{
    private Boss_Behavior bossBehavior;
    
    // We need our own reference to the Animator
    // because the base class 'animation' field is private
    private Animator bossAnimator; 

    // --- NEW ---
    [Header("Blocking Logic")]
    [Tooltip("How many hits it takes to trigger a block.")]
    public int hitsToBlock = 3; // Set this to 3 in the Inspector
    private int hitCounter = 0;   // Our new counter


    // 'new' keyword hides the base Start() method
    new void Start()
    {
        // --- Original Start() logic ---
        bossAnimator = GetComponent<Animator>(); // Get our own reference
        currentHealth = health;              // 'currentHealth' & 'health' are inherited

        // --- New Boss-specific logic ---
        bossBehavior = GetComponent<Boss_Behavior>();
        
        // --- NEW ---
        hitCounter = 0; // Ensure counter starts at 0
    }

    // 'new' keyword hides the base Update() method
    new void Update()
    {
        // --- Original Update() logic ---
        if (health < currentHealth)
        {
            currentHealth = health;
            bossAnimator.SetTrigger("Attacked"); // Use our animator reference

            // --- THIS IS THE MODIFIED PART ---
            
            // 1. Increment the hit counter
            hitCounter++;
            
            // 2. Check if the counter has reached the limit
            if (hitCounter >= hitsToBlock)
            {
                // 3. Reset the counter
                hitCounter = 0; 
                
                // 4. Tell the boss AI to block
                if (bossBehavior != null)
                {
                    bossBehavior.OnHit();
                }
            }
            // --- END MODIFIED PART ---
        }

        if (health <= 0)
        {
            bossAnimator.SetBool("isDead", true); // Use our animator reference
            Debug.Log("Enemy Dead");
        }
        // --- End of original logic ---
    }
}