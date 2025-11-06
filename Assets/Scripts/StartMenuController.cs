using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        // Loads the main game scene
        SceneManager.LoadScene("Main");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        // Stops play mode if running inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quits the application when built
        Application.Quit();
#endif
    }
}
