using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float currentHealth;

    private Animator enemyAnimator;
    private CapsuleCollider2D enemyCollider;
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
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