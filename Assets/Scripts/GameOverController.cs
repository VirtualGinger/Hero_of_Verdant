using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public void OnRetryClick()
    {
        // 1. Find the Player object (it must be tagged "Player")
        GameObject player = GameObject.FindWithTag("Player");

        // 2. If the player object exists (typically if marked DontDestroyOnLoad)
        if (player != null)
        {
            // Destroy the player object before starting the game over again
            Destroy(player);
            Debug.Log("Persistent Player object destroyed before retry.");
        }

        // 3. Load the main game scene
        SceneManager.LoadScene("StartScene");
    }

    public void OnReturnMainClick()
    {
        // Find and destroy the player object before returning to the main menu
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
            Debug.Log("Persistent Player object destroyed before returning to main menu.");
        }

#if UNITY_EDITOR
        // Stops play mode if running inside the Unity Editor
        // Note: You must ensure the necessary UnityEditor namespace is imported 
        // if this code block is not inside a #if UNITY_EDITOR block in a larger script.
        // Since we are in the main script file, this is fine.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SceneManager.LoadScene("StartScene");
#endif
    }
}