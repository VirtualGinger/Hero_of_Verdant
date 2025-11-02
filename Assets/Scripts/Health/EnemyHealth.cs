using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float currentHealth;
    private Animator animation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animation = GetComponent<Animator>();
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < currentHealth)
        {
            currentHealth = health;
            animation.SetTrigger("Attacked");
        }

        if (health <= 0)
        {
            animation.SetBool("isDead", true);
            Debug.Log("Enemy Dead");
        }
    }
}
