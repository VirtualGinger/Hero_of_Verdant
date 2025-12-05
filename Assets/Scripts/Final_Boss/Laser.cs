using UnityEngine;

public class Laser : MonoBehaviour
{
    public float damage = 1f;            // damage per hit
    public float tickRate = 0.1f;     // how often damage applies while player stays inside

    private Collider2D hitbox;
    private Animator animator;
    private float nextDamageTime;

    void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        if (hitbox != null)
            hitbox.enabled = false;

        gameObject.SetActive(false);
    }

    // Activate laser and point at target
    public void Activate(Vector2 targetPosition)
    {
        gameObject.SetActive(true);

        if (hitbox != null)
            hitbox.enabled = true;

        // Point laser toward target
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        nextDamageTime = Time.time;

        if (animator != null)
            animator.Play("LaserAnimation", -1, 0f); // restart animation
    }

    public void Deactivate()
    {
        if (hitbox != null)
            hitbox.enabled = false;

        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // --- ADD THIS LINE FOR DEBUGGING ---
        Debug.Log("Laser entered/staying in collision with: " + collision.gameObject.name);
        // ------------------------------------

        if (collision.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            // --- ADD THIS LINE TO CHECK TAG/TICK RATE ---
            Debug.Log("Player TAG detected. Applying damage now.");
            // ------------------------------------

            PlayerHealth hp = collision.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                nextDamageTime = Time.time + tickRate;
            }
        }
    }
}