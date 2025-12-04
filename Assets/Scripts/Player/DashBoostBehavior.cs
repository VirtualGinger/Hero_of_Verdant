using UnityEngine;

public class DashBoostBehavior : MonoBehaviour
{
    [Header("Configuration")]
    public float speedBoostAmount = 5f;
    public float boostDuration = 2f;

    private PlayerController player;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private bool wasDashing = false;

    private void Start()
    {
        // 1. Find the PlayerController on the PARENT (since this object is attached to Player)
        player = GetComponentInParent<PlayerController>();

        if (player == null)
        {
            Debug.LogError("DashBoostBehavior attached to something that isn't the player!");
            Destroy(gameObject); // Self-destruct if invalid
        }
    }

    private void Update()
    {
        if (player == null) return;

        // --- Logic: Detect when Dash FINISHES ---
        // We look for the moment IsDashing goes from TRUE to FALSE
        if (wasDashing && !player.IsDashing)
        {
            ApplyBoost();
        }

        // Store state for next frame
        wasDashing = player.IsDashing;

        // --- Logic: Handle Timer ---
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0f)
            {
                RemoveBoost();
            }
        }
    }

    private void ApplyBoost()
    {
        // Reset the timer so the boost lasts full duration
        boostTimer = boostDuration;

        // Only apply the speed addition if we haven't already
        if (!isBoosting)
        {
            player.moveSpeed += speedBoostAmount;
            isBoosting = true;
            Debug.Log("Speed Boost Activated!");
        }
    }

    private void RemoveBoost()
    {
        if (isBoosting)
        {
            player.moveSpeed -= speedBoostAmount;
            isBoosting = false;
            Debug.Log("Speed Boost Ended.");
        }
    }
    
    // Safety check: If this object is destroyed (e.g. player dies/scene change), remove the boost
    private void OnDestroy()
    {
        if (isBoosting && player != null)
        {
             player.moveSpeed -= speedBoostAmount;
        }
    }
}