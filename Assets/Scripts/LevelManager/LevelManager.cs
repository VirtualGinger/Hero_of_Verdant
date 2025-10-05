using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{    public static LevelManager Instance { get; private set; }

    [Tooltip("Drag the scene asset to load at startup from the Project window.")]
    public SceneReference startingScene;

    [Header("UI & Player")]
    public GameObject loadingScreen;
    public GameObject playerObject;

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
    public async void LoadLevel(string scenePath)
    {
        // 1. Always show loading screen and hide player initially.
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        if (playerObject != null)
        {
            playerObject.SetActive(false);
        }

        // 2. Unload the previous scene if one exists.
        if (!string.IsNullOrEmpty(_currentLevelName))
        {
            await SceneManager.UnloadSceneAsync(_currentLevelName);
        }

        // 3. Load the new scene.
        await SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        _currentLevelName = scenePath;
        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenePath));

        // 4. Check if the new scene has a SpawnPoint.
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

        if (spawnPoint != null) // --- THIS IS A PLAYABLE LEVEL ---
        {
            // Move the player to the spawn point.
            if (playerObject != null)
            {
                playerObject.transform.position = spawnPoint.transform.position;
                playerObject.transform.rotation = spawnPoint.transform.rotation;
            }

            // Activate the player.
            if (playerObject != null)
            {
                playerObject.SetActive(true);
            }
        }

        // 5. Hide the loading screen.
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    // Helper async method for loading
    public async Task LoadSceneAsync(string sceneName)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            await Task.Yield();
        }
    }

    // Helper async method for unloading
    public async Task UnloadSceneAsync(string sceneName)
    {
        var asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
        while (!asyncUnload.isDone)
        {
            await Task.Yield();
        }
    }
}