using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 5;
    public float moveSpeed = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called by ArrowLauncher to set direction
    public void SetDirection(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Arrow hit Player");
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
