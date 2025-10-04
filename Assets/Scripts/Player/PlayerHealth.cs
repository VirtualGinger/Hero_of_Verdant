using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public float invincibilityDuration = 1.5f; // How long the player is invincible after getting hit
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        // Get the SpriteRenderer component attached to this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        // If we are currently invincible, do nothing.
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            // Handle player death
            Debug.Log("Player has been defeated!");
            Destroy(gameObject);
        }
        else
        {
            // Start the invincibility and flashing effect
            StartCoroutine(InvincibilityFlash());
        }
    }

    private IEnumerator InvincibilityFlash()
    {
        // Set invincible flag
        isInvincible = true;

        float flashDelay = 0.15f; // How fast the sprite flashes
        float invincibilityTimer = 0f;

        // Loop for the duration of the invincibility
        while (invincibilityTimer < invincibilityDuration)
        {
            // Toggle the sprite's visibility
            spriteRenderer.enabled = !spriteRenderer.enabled;
            
            // Wait for the flash delay
            yield return new WaitForSeconds(flashDelay);
            
            // Increment the timer
            invincibilityTimer += flashDelay;
        }

        // Ensure the sprite is visible again at the end
        spriteRenderer.enabled = true;
        
        // Reset invincible flag
        isInvincible = false;
    }
}