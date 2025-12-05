using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float currentHealth;

    // 1. Add reference for the Collider2D
    private Animator enemyAnimator;
    private CapsuleCollider2D enemyCollider; // Reference to the collider

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        // 2. Get the collider component reference
        enemyCollider = GetComponent<CapsuleCollider2D>();

        currentHealth = health;

        if (enemyCollider == null)
        {
            Debug.LogError("EnemyHealth script requires a CapsuleCollider2D component on the same GameObject!");
        }
    }

    public void Heal(float amount)
    {
        if (health <= 0) return;

            health += amount;

        if (health > currentHealth)
            health = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < currentHealth)
        {
            currentHealth = health;
            enemyAnimator.SetTrigger("Attacked");
        }

        if (health <= 0)
        {
            Debug.Log("This enemey should be dead");
            if (enemyAnimator.GetBool("isDead") == false)
            {
                enemyAnimator.SetBool("isDead", true);
                Debug.Log("Setting isDead to true");
                // 3. Disable the collider when isDead is set to true
                if (enemyCollider != null)
                {
                    enemyCollider.enabled = false;
                }

                Debug.Log("Enemy Dead");
                Destroy(gameObject, 1.5f);
            }
        }
    }
}