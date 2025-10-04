using UnityEngine;

// This single script can be attached to any object to make it a hazard.
public class Hazard : MonoBehaviour
{
    [Tooltip("Amount of damage to deal to the player.")]
    public int damageAmount = 1;

    [Tooltip("The force of the pushback on the player.")]
    public float knockbackForce = 15f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object is the player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            Rigidbody2D playerRigidbody = other.GetComponent<Rigidbody2D>();

            if (playerHealth != null && playerRigidbody != null)
            {
                // Deal damage
                playerHealth.TakeDamage(damageAmount);

                // Calculate and apply knockback
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                playerRigidbody.linearVelocity = Vector2.zero; // Reset velocity for a consistent knockback
                playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}