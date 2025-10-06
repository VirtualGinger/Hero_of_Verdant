using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Call TakeDamage and pass THIS bush as the sender.
                playerHealth.TakeDamage(damageAmount, this.gameObject);
            }
        }
    }
}