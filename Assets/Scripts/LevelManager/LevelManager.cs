using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{    public static LevelManager Instance { get; private set; }

    [Tooltip("Drag the scene asset to load at startup from the Project window.")]
    public SceneReference startingScene;

    private string _currentLevelName;

    private void Awake()
    {
        // Setup the singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Check if a scene has been assigned in the Inspector
        if (startingScene != null)
        {
            LoadLevel(startingScene.ScenePath);
        }
        else
        {
            Debug.LogWarning("No starting scene was assigned in the LevelManager Inspector.");
        }
    }

    // This is the main function to call when changing levels
    public async void LoadLevel(string levelName)
    {
        // If a level is already loaded, unload it first
        if (!string.IsNullOrEmpty(_currentLevelName))
        {
            await UnloadSceneAsync(_currentLevelName);
        }

        // Load the new scene additively
        await LoadSceneAsync(levelName);
        _currentLevelName = levelName;

        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPoint != null) {
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnPoint.transform.position;
        }
    }

    // Helper async method for loading
    private async Task LoadSceneAsync(string sceneName)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            await Task.Yield();
        }
    }

    // Helper async method for unloading
    private async Task UnloadSceneAsync(string sceneName)
    {
        var asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
        while (!asyncUnload.isDone)
        {
            await Task.Yield();
        }
    }
}