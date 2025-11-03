using UnityEngine;

public class Player_Persist : MonoBehaviour
{
    private static Player_Persist instance;

    void Awake()
    {
        // If there's no instance yet, make this one persist
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another player already exists (from a previous scene), destroy this one
            Destroy(gameObject);
        }
    }
}
