using UnityEngine;

public class DashBoostBehavior : MonoBehaviour
{
    [Header("Configuration")]
    public float speedBoostAmount = 5f;
    public float boostDuration = 2f;

    // We store the amount we actually added to ensure we remove the EXACT same amount
    // even if you mess with the Inspector value during gameplay.
    private float addedAmount = 0f;
    
    private PlayerController player;
    private bool isBoosting = false;
    private float boostExpirationTime = 0f;
    private bool wasDashing = false;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();

        if (player == null)
        {
            // Safety check: destroy self if not on a player
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (player == null) return;

        // 1. Detect Dash Finish (Trailing State Check)
        // If we were dashing last frame, but are NOT dashing this frame...
        if (wasDashing && !player.IsDashing)
        {
            ApplyOrRefreshBoost();
        }

        // Update state for the next frame
        wasDashing = player.IsDashing;

        // 2. Check for Expiration
        // We use absolute time. This prevents the timer from getting "stuck".
        if (isBoosting && Time.time >= boostExpirationTime)
        {
            RemoveBoost();
        }
    }

    private void ApplyOrRefreshBoost()
    {
        // Always push the expiration time into the future
        boostExpirationTime = Time.time + boostDuration;

        // If we aren't boosting yet, add the speed
        if (!isBoosting)
        {
            addedAmount = speedBoostAmount; // Remember exactly what we added
            player.moveSpeed += addedAmount;
            isBoosting = true;
        }
        // If we ARE already boosting, we don't add speed again. 
        // We just updated 'boostExpirationTime' above, so the boost lasts longer.
    }

    private void RemoveBoost()
    {
        if (isBoosting)
        {
            player.moveSpeed -= addedAmount; // Remove exactly what we added
            isBoosting = false;
        }
    }

    // FAILSAFE: If this object is disabled or destroyed (Scene change, player death),
    // we MUST revert the speed immediately, or the player stays fast forever.
    private void OnDisable()
    {
        RemoveBoost();
    }

    private void OnDestroy()
    {
        RemoveBoost();
    }
}