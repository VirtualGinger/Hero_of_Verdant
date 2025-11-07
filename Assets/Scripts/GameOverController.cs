using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public void OnRetryClick()
    {
        // Loads the main game scene
        SceneManager.LoadScene("Hero_of_Verdant");
    }

    public void OnReturnMainClick()
    {
#if UNITY_EDITOR
        // Stops play mode if running inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SceneManager.LoadScene("StartScene");
#endif
    }
}
