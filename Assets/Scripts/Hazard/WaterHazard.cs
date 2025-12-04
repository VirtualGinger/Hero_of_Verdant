using UnityEngine;
using System.Collections; // Needed for Coroutines

public class WaterHazard : MonoBehaviour
{
    public int damageAmount = 1;

    // Define the delay duration
    private const float DamageDelay = 0.4f;

    // Variable to hold the active damage coroutine reference
    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any previous coroutine just in case (e.g., if the player briefly leaves and re-enters)
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }

            // Start the delayed damage coroutine
            damageCoroutine = StartCoroutine(ApplyDelayedDamage(other.gameObject));
        }
    }

    // Optional: Stop the coroutine if the player leaves the trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null; // Clear the reference
            }
        }
    }

    // --- COROUTINE FOR DELAYED DAMAGE ---
    IEnumerator ApplyDelayedDamage(GameObject playerObject)
    {
        // 1. Wait for the specified delay time (1.5 seconds)
        yield return new WaitForSeconds(DamageDelay);

        // 2. After the delay, apply the damage

        // Find the PlayerHealth component on the player object
        PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Call TakeDamage to apply the damage
            playerHealth.TakeDamage(damageAmount);
        }

        // Set the coroutine reference to null since it's finished
        damageCoroutine = null;
    }
}