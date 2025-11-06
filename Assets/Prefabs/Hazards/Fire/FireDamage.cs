using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public float damage;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.health -= damage;

                Debug.Log($"Hit Player! Player Health remaining: {playerHealth.health}");
            }
        }
    }
}
