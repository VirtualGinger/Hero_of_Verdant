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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1);

        if(health <= 0 && !isDead)
        { 
            isDead = true;
            StartCoroutine(DieSequence(0.3f));
        }
    }

    IEnumerator DieSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Player has died.");
        SceneManager.LoadScene("GameOver");
        //Destroy(gameObject);
    }
}
