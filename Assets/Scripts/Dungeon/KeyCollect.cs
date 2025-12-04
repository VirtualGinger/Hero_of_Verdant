using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class KeyCollect : MonoBehaviour
{
    public static KeyCollect instance;
    public TextMeshProUGUI text;
    public int keysCollected = 5;

    public string nextSceneName = "Hero_of_Verdant_3";
    private const float SceneLoadDelay = 1.5f;

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateText();
    }

    public void ChangeKeys(int amount)
    {
        keysCollected += amount;
        UpdateText();
    }

    private void UpdateText()
    {
        text.text = keysCollected.ToString();

        if(keysCollected <= 0)
        {
            StartCoroutine(DelayedSceneLoad());
        }
    }

    IEnumerator DelayedSceneLoad()
    {
  
        yield return new WaitForSeconds(SceneLoadDelay);

        SceneManager.LoadScene(nextSceneName);
    }
}
