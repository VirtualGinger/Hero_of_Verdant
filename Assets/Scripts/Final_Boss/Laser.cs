using UnityEngine;

public class Laser : MonoBehaviour
{
    public float lifeTime = 0.3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
