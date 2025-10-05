using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("Hero_Of_Verdant");
    }

    public void OnExitClick()
    {
#if Unity_Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
