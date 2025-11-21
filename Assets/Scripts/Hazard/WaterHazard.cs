using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterHazard : MonoBehaviour
{
    // --- Tile Damage Configuration ---
    [Header("Tile Damage Setup")]
    [Tooltip("The parent Grid component (usually the root object of the tilemaps).")]
    public Grid gameGrid;
    [Tooltip("The Tilemap layer (e.g., 'Hazard Layer') that contains damaging tiles.")]
    public Tilemap hazardTilemap;
    [Tooltip("The exact asset name of the tile that deals damage (e.g., 'Water').")]
    public string damagingTileName = "gentle forest v03_129";
    [Tooltip("Damage dealt PER SECOND while the Player stands on the hazard tile.")]
    public float tileDamagePerSecond = 5f;

    // --- Player References (Automatically Found) ---
    private Transform playerTransform;
    private PlayerHealth playerHealth; // Assumes your Player has a script named PlayerHealth

    void Start()
    {
        // Attempt to find the Player and their required components
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        // Error checking for setup
        if (playerTransform == null)
        {
            Debug.LogError("WaterHazard cannot find an object tagged 'Player'. Please ensure your player is tagged correctly.");
        }
        if (playerHealth == null && player != null)
        {
            Debug.LogError("WaterHazard found the Player, but it is missing the 'PlayerHealth' script. Cannot apply damage.");
        }
        if (gameGrid == null || hazardTilemap == null)
        {
            Debug.LogError("WaterHazard is missing Grid or Tilemap reference. Assign them in the Inspector!");
        }
    }

    void Update()
    {
        // Only run the check if we successfully found the player and their health component
        if (playerTransform != null && playerHealth != null)
        {
            CheckTileDamage();
        }
    }

    /// <summary>
    /// Converts the Player's world position to a grid cell and applies damage if the tile matches the hazard name.
    /// </summary>
    void CheckTileDamage()
    {
        if (gameGrid == null || hazardTilemap == null) return;

        // Adjust the sampling point slightly down to ensure we check the tile the player is standing on.
        // The value 0.1f is an attempt to sample slightly below the player's center origin.
        Vector3 checkPosition = playerTransform.position;
        checkPosition.y -= 0.1f;

        // Draw a green ray in the Scene view to show the exact point being checked.
        // This ray will persist for 0.1 seconds, enough to see it flicker.
        Debug.DrawRay(checkPosition, Vector3.up * 0.5f, Color.green, Time.deltaTime);

        // 1. Convert the adjusted world position to a Tilemap Cell position
        Vector3Int cellPosition = gameGrid.WorldToCell(checkPosition);

        // 2. Get the Tile object at that cell position on the hazard map
        TileBase tile = hazardTilemap.GetTile(cellPosition);

        // 3. Check if the tile exists and matches the damaging tile name
        if (tile != null && tile.name == damagingTileName)
        {
            // Calculate damage based on the time since last frame (damage per second)
            float damage = tileDamagePerSecond * Time.deltaTime;

            // Apply damage to the Player
            playerHealth.TakeDamage(damage);

            // Use Color.red for the debug message when damage is applied
            Debug.Log($"<color=red>Player standing on {tile.name}. Applying {damage:F2} damage. Cell: {cellPosition}</color>");
        }
        else
        {
            // Log that the check is running, but no damaging tile was found
            string tileName = (tile != null) ? tile.name : "NULL";
            Debug.Log($"Checking cell {cellPosition}. Found tile: {tileName}. Expected: {damagingTileName}.");
        }
    }

    
}