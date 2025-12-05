using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealthDragon : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private bool hasWarped = false;

    // Optional: wait this long before loading, set to 0 for instant
    [SerializeField] private float delaySeconds = 0f;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealthDragon requires EnemyHealth on the same GameObject.");
        }
    }

    void Update()
    {
        // If you prefer immediate warp the moment health hits 0, keep this:
        if (!hasWarped && enemyHealth != null && enemyHealth.health <= 0f)
        {
            // Mark as warped so we don't trigger multiple times from Update and OnDestroy
            hasWarped = true;
            LoadHeroScene();
        }
    }

    void OnDestroy()
    {
        // Fallback: if object is being destroyed due to death, ensure warp happens
        if (!hasWarped && enemyHealth != null && enemyHealth.health <= 0f)
        {
            hasWarped = true;
            LoadHeroScene();
        }
    }

    private void LoadHeroScene()
    {
        // If you want a slight delay for animation, use Invoke; otherwise set delaySeconds = 0
        if (delaySeconds > 0f)
        {
            Invoke(nameof(DoLoad), delaySeconds);
        }
        else
        {
            DoLoad();
        }
    }

    private void DoLoad()
    {
        // Ensure the scene exists in Build Settings and name matches exactly
        Debug.Log("Dragon defeated. Loading Hero_of_Verdant_4...");
        SceneManager.LoadScene("Hero_of_Verdant_4");
    }
}
