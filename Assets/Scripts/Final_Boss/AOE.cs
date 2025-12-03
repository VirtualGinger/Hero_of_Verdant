using UnityEngine;

public class AOE : MonoBehaviour
{
    public float lifeTime = 0.6f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}