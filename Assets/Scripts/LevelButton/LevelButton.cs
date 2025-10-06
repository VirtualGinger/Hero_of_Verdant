using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [Tooltip("Drag the scene asset you want this button to load.")]
    public SceneReference targetScene;

    public void LoadConfiguredScene()
    {
        if (targetScene != null && !string.IsNullOrEmpty(targetScene.ScenePath))
        {
            string scenePathToLoad = targetScene.ScenePath;

            LevelManager.Instance.LoadLevel(scenePathToLoad);
        }
        else
        {
            Debug.LogWarning("This button doesn't have a target scene assigned.", this);
        }
    }
}