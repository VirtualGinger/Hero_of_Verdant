using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// UnityEvent that can pass a GameObject. We need this for the OnHit event.
[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

public class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 3;
    private int currentHealth;

    // --- Events ---
    public GameObjectEvent OnHit; // Event to trigger knockback
    public UnityEvent OnDie;      // Event for when health reaches zero

    // --- Sprite Flashing ---
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float invincibilityDuration = 1f;
    private bool isInvincible = false;

    private void Start()
    {
        currentHealth = maxHealth;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void TakeDamage(int damage, GameObject sender)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        OnHit?.Invoke(sender); // Fire the OnHit event, passing the hazard as the sender.

        if (currentHealth <= 0)
        {
            OnDie?.Invoke();
        }
        else
        {
            StartCoroutine(InvincibilityFlash());
        }
    }
    
    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        for (float i = 0; i < invincibilityDuration; i += 0.2f)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}