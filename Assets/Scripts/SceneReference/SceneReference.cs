using UnityEngine;

// This single class lets you safely reference a scene asset.
[System.Serializable]
public class SceneReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    // This is the field you'll drag your scene into in the Inspector.
    [SerializeField] private UnityEditor.SceneAsset sceneAsset;
#endif

    // This stores the path to the scene, which is what's used in a build.
    [SerializeField] private string scenePath = string.Empty;

    // Public property to access the scene path from other scripts.
    public string ScenePath
    {
        get { return scenePath; }
    }

    // This function runs in the editor before serialization.
    // It takes the scene asset and saves its path into the scenePath string.
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            scenePath = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
        }
        else
        {
            scenePath = string.Empty;
        }
#endif
    }

    // This function is required by the interface but we don't need it.
    public void OnAfterDeserialize() { }
}