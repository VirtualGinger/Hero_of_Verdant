using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealthDragon : MonoBehaviour
{
    private bool hasWarped = false;

    // Called by EnemyHealth when the dragon dies
    public void HandleDragonDeath()
    {
        if (hasWarped) return;
        hasWarped = true;

        Debug.Log("Dragon defeated! Loading Hero_of_Verdant_4 in 1.5 seconds...");
        Invoke(nameof(DoWarp), 1.5f);
    }

    private void DoWarp()
    {
        SceneManager.LoadScene("Hero_of_Verdant_4");
    }
}
