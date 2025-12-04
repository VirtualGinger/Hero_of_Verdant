using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            if (KeyCollect.instance != null)
            {
                KeyCollect.instance.ChangeKeys(-1);
            }

            Destroy(gameObject);
        }
    }
}
