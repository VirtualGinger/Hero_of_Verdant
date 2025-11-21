using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 5;
    public Vector2 moveSpeed = new Vector2(3f, 0);

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.linearVelocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Arrow hit Player");
            // TODO: Apply damage to player health script here
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // arrows disappear when hitting terrain
        }
    }
}
