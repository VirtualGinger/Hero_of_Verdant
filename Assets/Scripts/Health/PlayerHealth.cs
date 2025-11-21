using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public Image HealthBar;

    private bool isDead = false;
    private PlayerPower playerPower; // reference to PlayerPower

    private void Start()
    {
        maxHealth = health;
        playerPower = GetComponent<PlayerPower>(); // assumes PlayerPower is on same GameObject
    }

    private void Update()
    {
        HealthBar.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1);

        if (health <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(DieSequence(0.3f));
        }
    }

    // Call this when something tries to damage the player
    public void TakeDamage(float amount)
    {
        // Ignore damage if invulnerable
        if (playerPower != null && playerPower.isInvulnerable) return;

        health -= amount;
    }

    private IEnumerator DieSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Player has died.");
        SceneManager.LoadScene("GameOver");
    }
}
