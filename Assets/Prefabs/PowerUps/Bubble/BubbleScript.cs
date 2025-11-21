using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    public int powerGain = 10; // how much power this bubble gives

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Add power to the player
            PlayerPower playerPower = collision.GetComponent<PlayerPower>();
            if (playerPower != null)
            {
                playerPower.power += powerGain;

                // Clamp so power never exceeds maxPower
                if (playerPower.power > playerPower.maxPower)
                {
                    playerPower.power = playerPower.maxPower;
                }
            }

            // Make bubble disappear
            Destroy(gameObject);
        }
    }
}
