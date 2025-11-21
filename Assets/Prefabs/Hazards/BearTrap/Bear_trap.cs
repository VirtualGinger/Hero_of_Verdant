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

            // Apply damage
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.health -= trapDamage;
            }

            // Trigger animation
            anim.SetBool("Activated", true);

            // Hold player in place
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                StartCoroutine(HoldPlayer(playerRb));
            }
        }
    }

    private IEnumerator HoldPlayer(Rigidbody2D playerRb)
    {
        // Freeze player movement
        RigidbodyConstraints2D originalConstraints = playerRb.constraints;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Wait for holdDuration seconds
        yield return new WaitForSeconds(holdDuration);

        // Release player (restore original constraints)
        playerRb.constraints = originalConstraints;
    }
}
