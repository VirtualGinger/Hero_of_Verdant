using UnityEngine;

public class DashBoostPickup : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Prefab containing the DashBoostBehavior script here")]
    public GameObject boostLogicPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. Spawn the logic object at the player's position
            GameObject newLogicObject = Instantiate(boostLogicPrefab, collision.transform.position, Quaternion.identity);

            // 2. Attach it to the player (make it a child)
            newLogicObject.transform.SetParent(collision.transform);
            
            // 3. Destroy this pickup from the world
            Destroy(gameObject);
        }
    }
}