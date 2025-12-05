using UnityEngine;
using TMPro;
using System.Collections;

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

            // 3. Update the TextMeshPro UI named "Warning"
            GameObject warningObj = GameObject.Find("Warning");
            if (warningObj != null)
            {
                TextMeshProUGUI warningText = warningObj.GetComponent<TextMeshProUGUI>();
                if (warningText != null)
                {
                    warningText.text = "Dash speed increased.";
                    // Run coroutine on the Warning object instead of this pickup
                    warningObj.GetComponent<MonoBehaviour>().StartCoroutine(ClearMessageAfterDelay(warningText, 3f));
                }
            }

            // 4. Destroy this pickup from the world
            Destroy(gameObject);
        }
    }

    private IEnumerator ClearMessageAfterDelay(TextMeshProUGUI textObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        textObj.text = string.Empty;
    }
}
