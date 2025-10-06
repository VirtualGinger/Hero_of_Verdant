using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("UI & Player References")]
    public GameObject loadingScreen;
    public GameObject playerObject;

    [Header("Starting Scene")]
    [Tooltip("Drag the first scene to load (e.g., a Main Menu) from the Project window.")]
    public SceneReference startingScene;

    private string _currentLevelName;

    void Awake()
    {
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
        if (startingScene != null && !string.IsNullOrEmpty(startingScene.ScenePath))
        {
            LoadLevel(startingScene.ScenePath);
        }
        else
        {
            Debug.LogWarning("No starting scene was assigned in the LevelManager Inspector.");
        }
    }

    public async void LoadLevel(string scenePath)
    {
        // 1. Show loading screen and hide player.
        if (loadingScreen != null) loadingScreen.SetActive(true);
        if (playerObject != null) playerObject.SetActive(false);

        // 2. Unload previous scene.
        if (!string.IsNullOrEmpty(_currentLevelName))
        {
            await SceneManager.UnloadSceneAsync(_currentLevelName);
        }

        // 3. Load new scene.
        await SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        _currentLevelName = scenePath;
        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenePath));

        // 4. Check if the scene is a playable level by looking for a SpawnPoint.
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

        if (spawnPoint != null) // This is a playable level.
        {
            Debug.Log("SpawnPoint found in '" + scenePath + "'. Spawning player.", spawnPoint);

            // Move and activate the player.
            if (playerObject != null)
            {
                playerObject.transform.position = spawnPoint.transform.position;
                playerObject.transform.rotation = spawnPoint.transform.rotation;
                playerObject.SetActive(true);
            }

            // Tell the camera to start following the player.
            CameraController mainCamera = FindObjectOfType<CameraController>();
            if (mainCamera != null)
            {
                mainCamera.StartFollowingPlayer();
            }
        }
        else // This is a menu or a scene without a spawn point.
        {
            Debug.Log("No SpawnPoint found in '" + scenePath + "'. Assuming it is a menu scene.");
        }

        // 5. Hide the loading screen.
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}