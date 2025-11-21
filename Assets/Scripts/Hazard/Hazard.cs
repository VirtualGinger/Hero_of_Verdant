using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Use PlayerHealth instead of Health
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Call TakeDamage so invulnerability logic is respected
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}