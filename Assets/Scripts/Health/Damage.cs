using UnityEngine;

public class Damage : MonoBehaviour
{
    public float damage;

    public bool hasHit = false;
    private bool firstTime = true;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player") && !hasHit)
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                if (firstTime)
                {
                    firstTime = false;
                    return;
                }
                playerHealth.health -= damage;

                hasHit = true;

                Debug.Log($"Hit Player! Player Health remaining: {playerHealth.health}");
            }
        }
    }

}