using UnityEngine;

public class EnemyHealthMage : MonoBehaviour
{
    // Public field for maximum health/starting health in the Inspector
    public float maxHealth = 10f;

    // CORE CHANGE: Use a backing field and a property for health
    private float _currentHealth;

    // This public property replaces the public 'health' variable 
    // and is what the PlayerController will interact with.
    public float health
    {
        get { return _currentHealth; }
        set
        {
            // 1. Calculate damage taken
            float damageTaken = _currentHealth - value;

            // 2. Set the new health value
            _currentHealth = value;

            // 3. Trigger hit animation immediately if damage was taken and not already dead
            if (damageTaken > 0 && _currentHealth > 0)
            {
                enemyAnimator.SetTrigger("Attacked");
            }

            // 4. Trigger death animation if health drops to zero or below
            if (_currentHealth <= 0 && enemyAnimator.GetBool("isDead") == false)
            {
                Die();
            }
        }
    }

    private Animator enemyAnimator;
    private CapsuleCollider2D enemyCollider; // Reference to the collider

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider2D>();

        // Initialize current health to maxHealth at start
        _currentHealth = maxHealth;

        if (enemyCollider == null)
        {
            Debug.LogError("EnemyHealth script requires a CapsuleCollider2D component on the same GameObject!");
        }
    }

    // REMOVED: The old logic is gone. The health setter handles all logic now.
    // void Update() {} 

    private void Die()
    {
        // Only set the boolean once to ensure the death animation starts
        enemyAnimator.SetBool("isDead", true);

        // Disable the collider
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // Add code to destroy the GameObject after the animation finishes (e.g., using Invoke or an animation event)
        Destroy(gameObject, 2f); // Destroy after 2 seconds

        Debug.Log("Enemy Dead");
    }
}