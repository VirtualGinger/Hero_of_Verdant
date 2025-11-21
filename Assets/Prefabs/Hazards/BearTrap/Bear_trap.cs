using UnityEngine;
using System.Collections;

public class Bear_trap : MonoBehaviour
{
    public int trapDamage = 10;
    public float holdDuration = 4f;
    private Animator anim;

    // Flag to ensure trap only triggers once
    private bool hasTriggered = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger once
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true; // mark trap as used

            // Apply damage immediately
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(trapDamage);
            }

            // Trigger animation
            anim.SetBool("Activated", true);

            // Start coroutine with delay before immobilizing
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                StartCoroutine(HoldPlayerWithDelay(playerRb));
            }
        }
    }

    private IEnumerator HoldPlayerWithDelay(Rigidbody2D playerRb)
    {
        // Wait 1 second before freezing player
        yield return new WaitForSeconds(0.3f);

        // Freeze player movement
        RigidbodyConstraints2D originalConstraints = playerRb.constraints;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Hold for holdDuration seconds
        yield return new WaitForSeconds(holdDuration);

        // Release player (restore original constraints)
        playerRb.constraints = originalConstraints;
    }
}
