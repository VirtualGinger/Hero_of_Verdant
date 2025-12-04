using UnityEngine;

public class PulsingHazard : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float minSize = 0.5f;
    public float maxSize = 3.5f;
    public float pulseSpeed = 1.0f;

    [Header("Damage")]
    public int damageAmount = 1;

    private void Update()
    {
        // 1. Calculate the scale value (bounces back and forth between 0 and 1)
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);

        // 2. Interpolate between minSize and maxSize based on 't'
        float currentSize = Mathf.Lerp(minSize, maxSize, t);

        // 3. Apply to the object's Scale
        transform.localScale = new Vector3(currentSize, currentSize, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Standard Damage Logic
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}